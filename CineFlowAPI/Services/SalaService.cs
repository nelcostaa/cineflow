using Cineflow.Data;
using Cineflow.Models;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Services;

public class SalaService : ISalaService
{
    private readonly AppDbContext _db;

    public SalaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Sala>> GetAllAsync()
    {
        return await _db.Salas
            .AsNoTracking()
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<Sala?> GetByIdAsync(int id)
    {
        return await _db.Salas
            .AsNoTracking()
            .Include(s => s.Sessoes)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Sala> CreateAsync(Sala sala)
    {
        if (sala.CapacidadeTotal <= 0)
            throw new ArgumentException("A capacidade total deve ser maior que zero.");

        var salaExistente = await _db.Salas
            .FirstOrDefaultAsync(s => s.Nome == sala.Nome);

        if (salaExistente != null)
            throw new InvalidOperationException($"Já existe uma sala com o nome '{sala.Nome}'.");

        _db.Salas.Add(sala);
        await _db.SaveChangesAsync();

        return sala;
    }

    public async Task<bool> UpdateAsync(int id, Sala sala)
    {
        var salaExistente = await _db.Salas.FindAsync(id);

        if (salaExistente == null)
            return false;

        if (sala.CapacidadeTotal <= 0)
            throw new ArgumentException("A capacidade total deve ser maior que zero.");

        if (salaExistente.Nome != sala.Nome)
        {
            var nomeJaExiste = await _db.Salas
                .AnyAsync(s => s.Nome == sala.Nome && s.Id != id);

            if (nomeJaExiste)
                throw new InvalidOperationException($"Já existe outra sala com o nome '{sala.Nome}'.");
        }

        salaExistente.Nome = sala.Nome;
        salaExistente.CapacidadeTotal = sala.CapacidadeTotal;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sala = await _db.Salas.FindAsync(id);

        if (sala == null)
            return false;

        // Verifica se existem sessões associadas
        var temSessoes = await _db.Sessoes.AnyAsync(s => s.SalaId == id);

        if (temSessoes)
            throw new InvalidOperationException("Não é possível deletar a sala pois existem sessões associadas a ela.");

        _db.Salas.Remove(sala);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<double> GetTaxaOcupacaoSalaAsync(int salaId, DateTime? de = null, DateTime? ate = null)
    {
        var sala = await _db.Salas.FindAsync(salaId);

        if (sala == null)
            throw new KeyNotFoundException("Sala não encontrada.");

        // Define período padrão se não informado
        de ??= DateTime.UtcNow.AddDays(-30);
        ate ??= DateTime.UtcNow;

        // Busca todas as sessões da sala no período
        var sessoes = await _db.Sessoes
            .Where(s => s.SalaId == salaId && s.HorarioInicio >= de && s.HorarioInicio <= ate)
            .Select(s => new { s.Id })
            .ToListAsync();

        if (sessoes.Count == 0)
            return 0;

        // Conta ingressos vendidos para essas sessões
        var sessaoIds = sessoes.Select(s => s.Id).ToList();
        var totalIngressosVendidos = await _db.Ingressos
            .CountAsync(i => sessaoIds.Contains(i.SessaoId));

        // Capacidade total possível = número de sessões × capacidade da sala
        var capacidadeTotal = sessoes.Count * sala.CapacidadeTotal;

        if (capacidadeTotal == 0)
            return 0;

        // Taxa de ocupação
        return (double)totalIngressosVendidos / capacidadeTotal * 100;
    }
}
