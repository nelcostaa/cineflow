using Cineflow.Models;

namespace Cineflow.Services;

public interface IIngressoService
{
    Task<List<Ingresso>> GetBySessaoAsync(int sessaoId);
    Task<Ingresso?> GetByIdAsync(int id);

    Task<Ingresso> ComprarAsync(int sessaoId, string lugarMarcado);

    Task<bool> CancelarAsync(int ingressoId);
}
