using Microsoft.EntityFrameworkCore;
using Cineflow.Models;

namespace Cineflow.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Filme> Filmes => Set<Filme>();
    public DbSet<Sala> Salas => Set<Sala>();
    public DbSet<Sessao> Sessoes => Set<Sessao>();
    public DbSet<Ingresso> Ingressos => Set<Ingresso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Filme>(e =>
        {
            e.Property(x => x.Titulo)
                .HasMaxLength(200)
                .IsRequired();

            // No model: string? => NÃO pode ser required
            e.Property(x => x.Genero)
                .HasMaxLength(80);

            e.HasIndex(x => x.IdTMDB)
                .IsUnique();

            e.Property(x => x.Popularity)
                .HasPrecision(10, 3);

            e.Property(x => x.VoteAverage)
                .HasPrecision(4, 3);

            e.Property(x => x.AtualizadoEm)
                .HasDefaultValueSql("GETUTCDATE()");


            e.Property(x => x.PosterPath).HasMaxLength(300);
            e.Property(x => x.BackdropPath).HasMaxLength(300);

            e.Property(x => x.TituloOriginal).HasMaxLength(200);
            e.Property(x => x.IdiomaOriginal).HasMaxLength(10);
        });

        modelBuilder.Entity<Sala>(e =>
        {
            e.Property(x => x.Nome)
                .HasMaxLength(80)
                .IsRequired();

            e.Property(x => x.CapacidadeTotal)
                .IsRequired();

            e.Ignore(x => x.LotacaoAtual);
        });

        modelBuilder.Entity<Sessao>(e =>
        {
            e.HasOne(x => x.Filme)
                .WithMany(x => x.Sessoes)
                .HasForeignKey(x => x.FilmeId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Sala)
                .WithMany(x => x.Sessoes)
                .HasForeignKey(x => x.SalaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.SalaId, x.HorarioInicio, x.HorarioFim });

            e.Property(x => x.HorarioInicio).IsRequired();
            e.Property(x => x.HorarioFim).IsRequired();
        });

        modelBuilder.Entity<Ingresso>(e =>
        {
            e.Property(x => x.LugarMarcado)
                .HasMaxLength(10)
                .IsRequired();

            e.Property(x => x.DataCompra)
                .HasDefaultValueSql("GETUTCDATE()");

            e.HasOne(x => x.Sessao)
                .WithMany(x => x.Ingressos)
                .HasForeignKey(x => x.SessaoId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.SessaoId, x.LugarMarcado })
                .IsUnique();
        });
    }
}
