using System.Text.Json.Serialization;

namespace Cineflow.Dtos.Tmdb;

public class TmdbNowPlayingResponseDto
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("results")]
    public List<TmdbMovieListItemDto> Results { get; set; } = new();
}
