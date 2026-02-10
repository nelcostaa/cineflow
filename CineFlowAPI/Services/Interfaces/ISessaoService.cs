using Cineflow.Models;

namespace Cineflow.Services;

public interface ISessaoService
{
    Task<List<Sessao>> GetAllAsync();
    Task<Sessao?> GetByIdAsync(int id);

    Task<Sessao> CreateAsync(Sessao sessao);
    Task<bool> UpdateAsync(int id, Sessao sessao);
    Task<bool> DeleteAsync(int id);

    Task<bool> HasConflitoAsync(int salaId, DateTime inicio, DateTime fim, int? ignoreSessaoId = null);

    Task<List<Sessao>> GetSessoesProximosDiasAsync(int dias = 7);
}
