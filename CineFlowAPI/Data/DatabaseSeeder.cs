using Cineflow.Models;
using Cineflow.Services;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Data;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly ITmdbService _tmdbService;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(AppDbContext context, ITmdbService tmdbService, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _tmdbService = tmdbService;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Filmes.AnyAsync() || await _context.Salas.AnyAsync())
        {
            _logger.LogInformation("Banco já possui dados, pulando seed inicial.");
            return;
        }

        _logger.LogInformation("🌱 Iniciando seed do banco de dados...");

        await SeedSalasAsync();

        await SeedFilmesAsync();

        _logger.LogInformation("✅ Seed concluído com sucesso!");
    }

    private async Task SeedSalasAsync()
    {
        var salas = new List<Sala>
        {
            new Sala
            {
                Nome = "Sala 1 - Premium",
                CapacidadeTotal = 120,
            },
            new Sala
            {
                Nome = "Sala 2 - Executiva",
                CapacidadeTotal = 80,
            },
            new Sala
            {
                Nome = "Sala 3 - Standard",
                CapacidadeTotal = 150,
            },
            new Sala
            {
                Nome = "Sala 4 - IMAX",
                CapacidadeTotal = 200,
            },
            new Sala
            {
                Nome = "Sala 5 - 3D",
                CapacidadeTotal = 100,
            }
        };

        _context.Salas.AddRange(salas);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✓ {salas.Count} salas criadas");
    }

    private async Task SeedFilmesAsync()
    {
        try
        {
            var generosMap = await _tmdbService.GetGenresMapAsync("pt-BR");
            var response = await _tmdbService.GetNowPlayingAsync(1, "pt-BR");

            if (response?.Results == null || !response.Results.Any())
            {
                _logger.LogWarning("Nenhum filme encontrado no TMDB");
                return;
            }

            var filmesParaImportar = response.Results.Take(10).ToList();
            var filmesImportados = 0;

            foreach (var tmdbFilme in filmesParaImportar)
            {
                if (await _context.Filmes.AnyAsync(f => f.IdTMDB == tmdbFilme.Id))
                    continue;

                int? runtime = null;
                try
                {
                    runtime = await _tmdbService.GetMovieRuntimeAsync(tmdbFilme.Id, "pt-BR");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Erro ao buscar runtime do filme {tmdbFilme.Title}: {ex.Message}");
                }

                DateTime? dataLancamento = null;
                if (!string.IsNullOrEmpty(tmdbFilme.ReleaseDate))
                {
                    if (DateTime.TryParse(tmdbFilme.ReleaseDate, out var parsedDate))
                    {
                        dataLancamento = parsedDate;
                    }
                }

                var generos = tmdbFilme.GenreIds
                    .Where(id => generosMap.ContainsKey(id))
                    .Select(id => generosMap[id])
                    .ToList();
                var generoString = generos.Any() ? string.Join(", ", generos) : "Não classificado";

                var filme = new Filme
                {
                    IdTMDB = tmdbFilme.Id,
                    Titulo = tmdbFilme.Title ?? "Sem título",
                    TituloOriginal = tmdbFilme.OriginalTitle,
                    IdiomaOriginal = tmdbFilme.OriginalLanguage,
                    DataLancamento = dataLancamento,
                    Sinopse = tmdbFilme.Overview,
                    DuracaoMinutos = runtime,
                    Genero = generoString,
                    PosterPath = tmdbFilme.PosterPath,
                    BackdropPath = tmdbFilme.BackdropPath,
                    VoteAverage = tmdbFilme.VoteAverage,
                    VoteCount = tmdbFilme.VoteCount,
                    Popularity = tmdbFilme.Popularity,
                    Adult = tmdbFilme.Adult,
                    Video = tmdbFilme.Video
                };

                _context.Filmes.Add(filme);
                filmesImportados++;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"✓ {filmesImportados} filmes importados do TMDB");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao importar filmes do TMDB: {ex.Message}");
        }
    }
}
