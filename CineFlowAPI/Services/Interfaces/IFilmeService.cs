using Cineflow.Models;

namespace Cineflow.Services;

public interface IFilmeService
{
    Task<List<Filme>> GetAllAsync();
    Task<Filme?> GetByIdAsync(int id);
    Task<Filme> CreateAsync(Filme filme);
    Task<bool> UpdateAsync(int id, Filme filme);
    Task<bool> DeleteAsync(int id);

    // Extra útil
    Task<Filme?> GetByTmdbIdAsync(int tmdbId);
}
