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

    public FilmesController(AppDbContext db, IFilmeService filmeService)
    {
        _db = db;
        _filmeService = filmeService;
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
}
