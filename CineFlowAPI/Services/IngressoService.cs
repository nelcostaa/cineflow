using Cineflow.Data;
using Cineflow.Models;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Services;

public class IngressoService : IIngressoService
{
    private readonly AppDbContext _db;
    public IngressoService(AppDbContext db) => _db = db;

    public async Task<List<Ingresso>> GetBySessaoAsync(int sessaoId)
        => await _db.Ingressos
            .AsNoTracking()
            .Where(i => i.SessaoId == sessaoId)
            .OrderBy(i => i.LugarMarcado)
            .ToListAsync();

    public async Task<Ingresso?> GetByIdAsync(int id)
        => await _db.Ingressos.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

    public async Task<Ingresso> ComprarAsync(int sessaoId, string lugarMarcado, decimal preco)
    {
        lugarMarcado = lugarMarcado.Trim();
        if (string.IsNullOrWhiteSpace(lugarMarcado))
            throw new ArgumentException("LugarMarcado é obrigatório.");

        if (preco <= 0)
            throw new ArgumentException("O preço deve ser maior que zero.");

        var sessao = await _db.Sessoes
            .Include(s => s.Sala)
            .FirstOrDefaultAsync(s => s.Id == sessaoId);

        if (sessao is null)
            throw new KeyNotFoundException("Sessão não encontrada.");

        var vendidos = await _db.Ingressos.CountAsync(i => i.SessaoId == sessaoId);
        if (vendidos >= sessao.Sala.CapacidadeTotal)
            throw new InvalidOperationException("Sessão lotada.");

        var ingresso = new Ingresso
        {
            SessaoId = sessaoId,
            LugarMarcado = lugarMarcado,
            Preco = preco,
            DataCompra = DateTime.UtcNow
        };

        _db.Ingressos.Add(ingresso);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Aqui normalmente pega violação do índice único (SessaoId, LugarMarcado)
            throw new InvalidOperationException("Esse assento já foi vendido para essa sessão.", ex);
        }

        return ingresso;
    }

    public async Task<bool> CancelarAsync(int ingressoId)
    {
        var ingresso = await _db.Ingressos.FirstOrDefaultAsync(i => i.Id == ingressoId);
        if (ingresso is null) return false;

        _db.Ingressos.Remove(ingresso);
        await _db.SaveChangesAsync();
        return true;
    }
}

