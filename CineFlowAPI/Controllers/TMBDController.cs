using Cineflow.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/tmdb")]
public class TmdbController : ControllerBase
{
    private readonly ITmdbService _tmdb;
    public TmdbController(ITmdbService tmdb) => _tmdb = tmdb;

    [HttpGet("filmes/{tmdbId:int}")]
    public async Task<IActionResult> GetDetalhes(int tmdbId)
    {
        var json = await _tmdb.GetMovieDetailsRawAsync(tmdbId, "pt-BR");
        if (json is null) return StatusCode(502, "Falha ao consultar TMDB (movie details).");

        return Content(json, "application/json");
    }
}
