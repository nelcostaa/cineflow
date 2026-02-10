using Cineflow.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/tmdb")]
public class TmdbController : ControllerBase
{
    private readonly ITmdbService _tmdb;

    public TmdbController(ITmdbService tmdb)
    {
        _tmdb = tmdb;
    }

    [HttpGet("now-playing")]
    public async Task<IActionResult> GetNowPlaying()
    {
        var result = await _tmdb.GetNowPlayingAsync();

        if (result is null)
            return StatusCode(502, "Erro ao consultar TMDB.");

        return Ok(result);
    }
}

