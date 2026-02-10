using Cineflow.Data;
using Cineflow.Models;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Services;

public class SessaoService : ISessaoService
{
    private readonly AppDbContext _db;
    public SessaoService(AppDbContext db) => _db = db;

    public async Task<List<Sessao>> GetAllAsync()
        => await _db.Sessoes
            .AsNoTracking()
            .Include(s => s.Filme)
            .Include(s => s.Sala)
            .ToListAsync();

    public async Task<Sessao?> GetByIdAsync(int id)
        => await _db.Sessoes
            .AsNoTracking()
            .Include(s => s.Filme)
            .Include(s => s.Sala)
            .Include(s => s.Ingressos)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Sessao> CreateAsync(Sessao sessao)
    {
        if (sessao.HorarioFim <= sessao.HorarioInicio)
            throw new ArgumentException("HorarioFim deve ser maior que HorarioInicio.");

        var conflito = await HasConflitoAsync(sessao.SalaId, sessao.HorarioInicio, sessao.HorarioFim);
        if (conflito)
            throw new InvalidOperationException("Conflito de horário: já existe sessão nessa sala no intervalo informado.");

        // valida FK (opcional, mas bom)
        var filmeOk = await _db.Filmes.AnyAsync(f => f.Id == sessao.FilmeId);
        var salaOk = await _db.Salas.AnyAsync(s => s.Id == sessao.SalaId);
        if (!filmeOk) throw new KeyNotFoundException("Filme não encontrado.");
        if (!salaOk) throw new KeyNotFoundException("Sala não encontrada.");

        _db.Sessoes.Add(sessao);
        await _db.SaveChangesAsync();
        return sessao;
    }

    public async Task<bool> UpdateAsync(int id, Sessao sessao)
    {
        if (id != sessao.Id) throw new ArgumentException("Id da URL difere do body.");
        if (sessao.HorarioFim <= sessao.HorarioInicio)
            throw new ArgumentException("HorarioFim deve ser maior que HorarioInicio.");

        var existing = await _db.Sessoes.FirstOrDefaultAsync(s => s.Id == id);
        if (existing is null) return false;

        var conflito = await HasConflitoAsync(sessao.SalaId, sessao.HorarioInicio, sessao.HorarioFim, ignoreSessaoId: id);
        if (conflito)
            throw new InvalidOperationException("Conflito de horário: já existe sessão nessa sala no intervalo informado.");

        existing.FilmeId = sessao.FilmeId;
        existing.SalaId = sessao.SalaId;
        existing.HorarioInicio = sessao.HorarioInicio;
        existing.HorarioFim = sessao.HorarioFim;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Sessoes.FirstOrDefaultAsync(s => s.Id == id);
        if (existing is null) return false;

        _db.Sessoes.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasConflitoAsync(int salaId, DateTime inicio, DateTime fim, int? ignoreSessaoId = null)
    {
        var query = _db.Sessoes.AsNoTracking().Where(s => s.SalaId == salaId);

        if (ignoreSessaoId.HasValue)
            query = query.Where(s => s.Id != ignoreSessaoId.Value);

        // CONFLITO: novoInicio < existenteFim && novoFim > existenteInicio
        return await query.AnyAsync(s => inicio < s.HorarioFim && fim > s.HorarioInicio);
    }

    public async Task<List<Sessao>> GetSessoesProximosDiasAsync(int dias = 7)
    {
        var inicio = DateTime.UtcNow;
        var fim = inicio.AddDays(dias);

        return await _db.Sessoes
            .AsNoTracking()
            .Include(s => s.Filme)
            .Include(s => s.Sala)
            .Where(s => s.HorarioInicio >= inicio && s.HorarioInicio <= fim)
            .OrderBy(s => s.HorarioInicio)
            .ToListAsync();
    }
}
