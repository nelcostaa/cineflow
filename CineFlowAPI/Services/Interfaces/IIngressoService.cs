using Cineflow.Dtos;
using Cineflow.Models;

namespace Cineflow.Services;

public interface IIngressoService
{
    Task<List<Ingresso>> GetBySessaoAsync(int sessaoId);
    Task<Ingresso?> GetByIdAsync(int id);
    Task<AssentosDisponiveisDto> GetAssentosDisponiveisAsync(int sessaoId);

    Task<Ingresso> ComprarAsync(int sessaoId, string lugarMarcado, decimal preco, string tipoIngresso = "Inteira");

    Task<bool> CancelarAsync(int ingressoId);
}