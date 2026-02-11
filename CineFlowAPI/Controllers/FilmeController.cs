using Cineflow.Data;
using Cineflow.Models;
using Cineflow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFilmeService _filmeService;
    private readonly ITmdbService _tmdbService;

    public FilmesController(AppDbContext db, IFilmeService filmeService, ITmdbService tmdbService)
    {
        _db = db;
        _filmeService = filmeService;
        _tmdbService = tmdbService;
    }

    //GET /api/filmes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var filmes = await _filmeService.GetAllAsync();
            return Ok(filmes);
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro ao buscar filmes.");
        }
    }

    //GET /api/filmes/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var filme = await _filmeService.GetByIdAsync(id);

        if (filme is null)
            return NotFound(new { message = "Filme não encontrado." });

        return Ok(filme);
    }

    //POST /api/filmes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Filme filme)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var novoFilme = await _filmeService.CreateAsync(filme);
            return CreatedAtAction(nameof(GetById), new { id = novoFilme.Id }, novoFilme);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar filme.", error = ex.Message });
        }
    }

    //PUT /api/filmes/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Filme filme)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var sucesso = await _filmeService.UpdateAsync(id, filme);

            if (!sucesso)
                return NotFound(new { message = "Filme não encontrado." });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao atualizar filme.", error = ex.Message });
        }
    }

    //DELETE /api/filmes/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var sucesso = await _filmeService.DeleteAsync(id);

            if (!sucesso)
                return NotFound(new { message = "Filme não encontrado." });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao deletar filme.", error = ex.Message });
        }
    }

    //POST /api/filmes/importar-em-cartaz
    [HttpPost("importar-em-cartaz")]
    public async Task<IActionResult> ImportarEmCartaz([FromQuery] int quantidadePaginas = 1)
    {
        try
        {
            var filmesImportados = new List<Filme>();
            var generos = await _tmdbService.GetGenresMapAsync();

            for (int page = 1; page <= quantidadePaginas; page++)
            {
                var response = await _tmdbService.GetNowPlayingAsync(page);
                if (response == null || response.Results == null) continue;

                foreach (var tmdbFilme in response.Results)
                {
                    var existente = await _filmeService.GetByTmdbIdAsync(tmdbFilme.Id);
                    if (existente != null) continue;

                    var generosNomes = tmdbFilme.GenreIds?
                        .Where(id => generos.ContainsKey(id))
                        .Select(id => generos[id])
                        .ToList() ?? new List<string>();

                    var filme = new Filme
                    {
                        IdTMDB = tmdbFilme.Id,
                        Titulo = tmdbFilme.Title ?? "Sem título",
                        TituloOriginal = tmdbFilme.OriginalTitle,
                        IdiomaOriginal = tmdbFilme.OriginalLanguage,
                        DataLancamento = DateTime.TryParse(tmdbFilme.ReleaseDate, out var data) ? data : null,
                        Sinopse = tmdbFilme.Overview,
                        Genero = string.Join(", ", generosNomes),
                        PosterPath = tmdbFilme.PosterPath,
                        BackdropPath = tmdbFilme.BackdropPath,
                        VoteAverage = tmdbFilme.VoteAverage,
                        VoteCount = tmdbFilme.VoteCount,
                        Popularity = tmdbFilme.Popularity,
                        Adult = tmdbFilme.Adult,
                        Video = tmdbFilme.Video
                    };

                    var runtime = await _tmdbService.GetMovieRuntimeAsync(tmdbFilme.Id);
                    if (runtime.HasValue)
                    {
                        filme.DuracaoMinutos = runtime.Value;
                    }

                    var novoFilme = await _filmeService.CreateAsync(filme);
                    filmesImportados.Add(novoFilme);
                }
            }

            return Ok(new
            {
                message = $"{filmesImportados.Count} filmes importados com sucesso.",
                filmes = filmesImportados
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao importar filmes.", error = ex.Message });
        }
    }
}
