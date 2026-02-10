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
            e.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
            e.Property(x => x.Genero).HasMaxLength(80).IsRequired();
            e.HasIndex(x => x.IdTMDB).IsUnique();

            e.Property(x => x.Popularity).HasPrecision(10, 3);
            e.Property(x => x.VoteAverage).HasPrecision(4, 3);
        });

        modelBuilder.Entity<Sala>(e =>
        {
            e.Property(x => x.Nome).HasMaxLength(80).IsRequired();
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

            e.HasIndex(x => new { x.SalaId, x.HorarioInicio });
        });

        modelBuilder.Entity<Ingresso>(e =>
        {
            e.Property(x => x.LugarMarcado).HasMaxLength(10).IsRequired();

            e.HasOne(x => x.Sessao)
             .WithMany(x => x.Ingressos)
             .HasForeignKey(x => x.SessaoId)
             .OnDelete(DeleteBehavior.Cascade);
             
            e.HasIndex(x => new { x.SessaoId, x.LugarMarcado }).IsUnique();
        });
    }
}
