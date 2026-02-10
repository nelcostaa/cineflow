using Cineflow.Dtos.Tmdb;

namespace Cineflow.Services;

public interface ITmdbService
{
    Task<TmdbNowPlayingResponseDto?> GetNowPlayingAsync(
        int page = 1,
        string language = "pt-BR"
    );

    Task<Dictionary<int, string>> GetGenresMapAsync(
        string language = "pt-BR"
    );

    Task<string?> GetMovieDetailsRawAsync(
        int tmdbId,
        string language = "pt-BR"
    );
}
