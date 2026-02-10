using System.Text.Json;
using Cineflow.Dtos.Tmdb;
using RestSharp;

namespace Cineflow.Services;

public class TmdbService
{
    private readonly string _token;
    private readonly RestClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TmdbService(IConfiguration config)
    {
        _token = config["Tmdb:Token"]
            ?? throw new Exception("Token da TMDB não configurado (Tmdb:Token).");

        _client = new RestClient("https://api.themoviedb.org/3/");
    }

    public async Task<TmdbNowPlayingResponseDto?> GetNowPlayingAsync(int page = 1, string language = "pt-BR")
    {
        var request = new RestRequest("movie/now_playing");
        request.AddQueryParameter("language", language);
        request.AddQueryParameter("page", page.ToString());
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {_token}");

        var response = await _client.ExecuteAsync(request);
        if (!response.IsSuccessful || response.Content is null) return null;

        return JsonSerializer.Deserialize<TmdbNowPlayingResponseDto>(response.Content, _jsonOptions);
    }

    public async Task<Dictionary<int, string>> GetGenresMapAsync(string language = "pt-BR")
    {
        var request = new RestRequest("genre/movie/list");
        request.AddQueryParameter("language", language);
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {_token}");

        var response = await _client.ExecuteAsync(request);
        if (!response.IsSuccessful || response.Content is null) return new();

        var dto = JsonSerializer.Deserialize<TmdbGenresResponseDto>(response.Content, _jsonOptions);
        return dto?.Genres.ToDictionary(g => g.Id, g => g.Name) ?? new();
    }

    public async Task<string?> GetMovieDetailsRawAsync(int tmdbId, string language = "pt-BR")
    {
        var request = new RestRequest($"movie/{tmdbId}");
        request.AddQueryParameter("language", language);
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {_token}");

        var response = await _client.ExecuteAsync(request);
        if (!response.IsSuccessful || response.Content is null) return null;

        return response.Content;
    }
}
