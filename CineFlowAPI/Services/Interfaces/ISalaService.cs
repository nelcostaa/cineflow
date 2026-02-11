using Cineflow.Models;

namespace Cineflow.Services;

public interface ISalaService
{
    Task<List<Sala>> GetAllAsync();
    Task<Sala?> GetByIdAsync(int id);
    Task<Sala> CreateAsync(Sala sala);
    Task<bool> UpdateAsync(int id, Sala sala);
    Task<bool> DeleteAsync(int id);

    Task<double> GetTaxaOcupacaoSalaAsync(int salaId, DateTime? de = null, DateTime? ate = null);
}
