using Cineflow.Data;
using Cineflow.Dtos;
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

    public async Task<AssentosDisponiveisDto> GetAssentosDisponiveisAsync(int sessaoId)
    {
        var sessao = await _db.Sessoes
            .Include(s => s.Sala)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sessaoId);

        if (sessao is null)
            throw new KeyNotFoundException("Sessão não encontrada.");

        var ingressosAtivos = await _db.Ingressos
            .Where(i => i.SessaoId == sessaoId && i.StatusIngresso == "Ativo")
            .AsNoTracking()
            .ToListAsync();

        var assentosOcupados = ingressosAtivos.Select(i => i.LugarMarcado).ToList();

        return new AssentosDisponiveisDto
        {
            SessaoId = sessaoId,
            CapacidadeTotal = sessao.Sala.CapacidadeTotal,
            IngressosVendidos = ingressosAtivos.Count,
            LugaresDisponiveis = sessao.Sala.CapacidadeTotal - ingressosAtivos.Count,
            AssentosOcupados = assentosOcupados,
            PrecoInteira = sessao.PrecoBase,
            PrecoMeia = sessao.PrecoBase / 2
        };
    }

    public async Task<Ingresso> ComprarAsync(int sessaoId, string lugarMarcado, decimal preco, string tipoIngresso = "Inteira")
    {
        lugarMarcado = lugarMarcado.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(lugarMarcado))
            throw new ArgumentException("LugarMarcado é obrigatório.");

        if (preco <= 0)
            throw new ArgumentException("O preço deve ser maior que zero.");

        if (tipoIngresso != "Inteira" && tipoIngresso != "Meia")
            throw new ArgumentException("Tipo de ingresso deve ser 'Inteira' ou 'Meia'.");

        var sessao = await _db.Sessoes
            .Include(s => s.Sala)
            .FirstOrDefaultAsync(s => s.Id == sessaoId);

        if (sessao is null)
            throw new KeyNotFoundException("Sessão não encontrada.");

        // Verificar se o assento já está ocupado
        var assentoJaOcupado = await _db.Ingressos
            .AnyAsync(i => i.SessaoId == sessaoId &&
                          i.LugarMarcado == lugarMarcado &&
                          i.StatusIngresso == "Ativo");

        if (assentoJaOcupado)
            throw new InvalidOperationException($"O assento {lugarMarcado} já está ocupado.");

        var vendidos = await _db.Ingressos
            .CountAsync(i => i.SessaoId == sessaoId && i.StatusIngresso == "Ativo");

        if (vendidos >= sessao.Sala.CapacidadeTotal)
            throw new InvalidOperationException("Sessão lotada.");

        var ingresso = new Ingresso
        {
            SessaoId = sessaoId,
            LugarMarcado = lugarMarcado,
            Preco = preco,
            DataCompra = DateTime.UtcNow,
            StatusIngresso = "Ativo"
        };

        _db.Ingressos.Add(ingresso);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Esse assento já foi vendido para essa sessão.", ex);
        }

        return ingresso;
    }

    public async Task<bool> CancelarAsync(int ingressoId)
    {
        var ingresso = await _db.Ingressos.FirstOrDefaultAsync(i => i.Id == ingressoId);
        if (ingresso is null) return false;

        ingresso.StatusIngresso = "Cancelado";
        await _db.SaveChangesAsync();
        return true;
    }
}

