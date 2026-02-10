using Cineflow.Data;
using Cineflow.Models;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Services;

public class FilmeService : IFilmeService
{
    private readonly AppDbContext _db;

    public FilmeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Filme>> GetAllAsync()
    {
        return await _db.Filmes
            .Include(f => f.Sessoes)
            .OrderByDescending(f => f.AtualizadoEm)
            .ToListAsync();
    }

    public async Task<Filme?> GetByIdAsync(int id)
    {
        return await _db.Filmes
            .Include(f => f.Sessoes)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Filme?> GetByTmdbIdAsync(int tmdbId)
    {
        return await _db.Filmes
            .Include(f => f.Sessoes)
            .FirstOrDefaultAsync(f => f.IdTMDB == tmdbId);
    }

    public async Task<Filme> CreateAsync(Filme filme)
    {
        var filmeExistente = await _db.Filmes
            .FirstOrDefaultAsync(f => f.IdTMDB == filme.IdTMDB);

        if (filmeExistente != null)
        {
            throw new InvalidOperationException($"Já existe um filme cadastrado com o IdTMDB {filme.IdTMDB}.");
        }

        filme.AtualizadoEm = DateTime.UtcNow;

        _db.Filmes.Add(filme);
        await _db.SaveChangesAsync();

        return filme;
    }

    public async Task<bool> UpdateAsync(int id, Filme filme)
    {
        var filmeExistente = await _db.Filmes.FindAsync(id);

        if (filmeExistente == null)
            return false;

        if (filmeExistente.IdTMDB != filme.IdTMDB)
        {
            var tmdbJaExiste = await _db.Filmes
                .AnyAsync(f => f.IdTMDB == filme.IdTMDB && f.Id != id);

            if (tmdbJaExiste)
            {
                throw new InvalidOperationException($"Já existe outro filme cadastrado com o IdTMDB {filme.IdTMDB}.");
            }
        }

        filmeExistente.IdTMDB = filme.IdTMDB;
        filmeExistente.Titulo = filme.Titulo;
        filmeExistente.TituloOriginal = filme.TituloOriginal;
        filmeExistente.IdiomaOriginal = filme.IdiomaOriginal;
        filmeExistente.DataLancamento = filme.DataLancamento;
        filmeExistente.Sinopse = filme.Sinopse;
        filmeExistente.DuracaoMinutos = filme.DuracaoMinutos;
        filmeExistente.Genero = filme.Genero;
        filmeExistente.PosterPath = filme.PosterPath;
        filmeExistente.BackdropPath = filme.BackdropPath;
        filmeExistente.VoteAverage = filme.VoteAverage;
        filmeExistente.VoteCount = filme.VoteCount;
        filmeExistente.Popularity = filme.Popularity;
        filmeExistente.Adult = filme.Adult;
        filmeExistente.Video = filme.Video;
        filmeExistente.AtualizadoEm = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var filme = await _db.Filmes.FindAsync(id);

        if (filme == null)
            return false;

        var temSessoes = await _db.Sessoes.AnyAsync(s => s.FilmeId == id);

        if (temSessoes)
        {
            throw new InvalidOperationException("Não é possível deletar o filme pois existem sessões associadas a ele.");
        }

        _db.Filmes.Remove(filme);
        await _db.SaveChangesAsync();

        return true;
    }
}
