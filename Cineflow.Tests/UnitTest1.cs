using Cineflow.Data;
using Cineflow.Models;
using Cineflow.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cineflow.Tests;

public class SessaoServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task NaoDeveCriarSessoesNoMesmoHorarioEMesmaSala()
    {
        var context = GetInMemoryDbContext();
        var sessaoService = new SessaoService(context);

        var filme = new Filme
        {
            IdTMDB = 1,
            Titulo = "Filme Teste",
            Sinopse = "Sinopse teste",
            DuracaoMinutos = 120,
            Genero = "Ação"
        };

        var sala = new Sala
        {
            Nome = "Sala 1",
            CapacidadeTotal = 100
        };

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        await context.SaveChangesAsync();

        var horarioInicio = DateTime.Now.AddHours(2);
        var horarioFim = horarioInicio.AddHours(2);

        var sessao1 = new Sessao
        {
            FilmeId = filme.Id,
            SalaId = sala.Id,
            HorarioInicio = horarioInicio,
            HorarioFim = horarioFim,
            PrecoBase = 25.00m,
            Status = "Ativa"
        };

        var primeiraSessao = await sessaoService.CreateAsync(sessao1);

        var sessao2 = new Sessao
        {
            FilmeId = filme.Id,
            SalaId = sala.Id,
            HorarioInicio = horarioInicio.AddMinutes(30),
            HorarioFim = horarioFim.AddMinutes(30),
            PrecoBase = 25.00m,
            Status = "Ativa"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await sessaoService.CreateAsync(sessao2);
        });
    }

    [Fact]
    public async Task DeveCriarSessoesNoMesmoHorarioEmSalasDiferentes()
    {
        var context = GetInMemoryDbContext();
        var sessaoService = new SessaoService(context);

        var filme = new Filme
        {
            IdTMDB = 1,
            Titulo = "Filme Teste",
            Sinopse = "Sinopse teste",
            DuracaoMinutos = 120,
            Genero = "Ação"
        };

        var sala1 = new Sala { Nome = "Sala 1", CapacidadeTotal = 100 };
        var sala2 = new Sala { Nome = "Sala 2", CapacidadeTotal = 150 };

        context.Filmes.Add(filme);
        context.Salas.AddRange(sala1, sala2);
        await context.SaveChangesAsync();

        var horarioInicio = DateTime.Now.AddHours(2);
        var horarioFim = horarioInicio.AddHours(2);

        var sessao1 = new Sessao
        {
            FilmeId = filme.Id,
            SalaId = sala1.Id,
            HorarioInicio = horarioInicio,
            HorarioFim = horarioFim,
            PrecoBase = 25.00m,
            Status = "Ativa"
        };

        var sessao2 = new Sessao
        {
            FilmeId = filme.Id,
            SalaId = sala2.Id,
            HorarioInicio = horarioInicio,
            HorarioFim = horarioFim,
            PrecoBase = 25.00m,
            Status = "Ativa"
        };

        var primeiraSessao = await sessaoService.CreateAsync(sessao1);
        var segundaSessao = await sessaoService.CreateAsync(sessao2);

        Assert.NotNull(primeiraSessao);
        Assert.NotNull(segundaSessao);
        Assert.NotEqual(primeiraSessao.SalaId, segundaSessao.SalaId);
    }
}

public class IngressoServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task NaoDeveComprarIngressoParaSessaoLotada()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var ingressoService = new IngressoService(context);

        var filme = new Filme
        {
            IdTMDB = 1,
            Titulo = "Filme Teste",
            Sinopse = "Sinopse teste",
            DuracaoMinutos = 120,
            Genero = "Ação"
        };

        var sala = new Sala
        {
            Nome = "Sala Mini",
            CapacidadeTotal = 2 // Capacidade pequena para facilitar o teste
        };

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        await context.SaveChangesAsync();

        var sessao = new Sessao
        {
            FilmeId = filme.Id,
            SalaId = sala.Id,
            HorarioInicio = DateTime.Now.AddHours(2),
            HorarioFim = DateTime.Now.AddHours(4),
            PrecoBase = 25.00m,
            Status = "Ativa"
        };

        context.Sessoes.Add(sessao);
        await context.SaveChangesAsync();

        // Act - Comprar ingressos até lotar
        var ingresso1 = await ingressoService.ComprarAsync(sessao.Id, "A1", 25.00m);
        var ingresso2 = await ingressoService.ComprarAsync(sessao.Id, "A2", 25.00m);

        // Assert - Tentar comprar mais um ingresso (deve falhar)
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await ingressoService.ComprarAsync(sessao.Id, "A3", 25.00m);
        });
    }

    [Fact]
    public async Task NaoDeveComprarIngressoParaAssentoJaOcupado()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var ingressoService = new IngressoService(context);

        var filme = new Filme
        {
            IdTMDB = 1,
            Titulo = "Filme Teste",
            Sinopse = "Sinopse teste",
            DuracaoMinutos = 120,
            Genero = "Ação"
        };

        var sala = new Sala
        {
            Nome = "Sala Teste",
            CapacidadeTotal = 100
        };

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        await context.SaveChangesAsync();

        var sessao = new Sessao
        {
            FilmeId = filme.Id,
            SalaId = sala.Id,
            HorarioInicio = DateTime.Now.AddHours(2),
            HorarioFim = DateTime.Now.AddHours(4),
            PrecoBase = 25.00m,
            Status = "Ativa"
        };

        context.Sessoes.Add(sessao);
        await context.SaveChangesAsync();

        // Act - Comprar ingresso para assento A1
        var ingresso1 = await ingressoService.ComprarAsync(sessao.Id, "A1", 25.00m);

        // Assert - Tentar comprar novamente para o mesmo assento (deve falhar)
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await ingressoService.ComprarAsync(sessao.Id, "A1", 25.00m);
        });
    }

    [Fact]
    public async Task DeveCalcularPrecoMeiaCorretamente()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var ingressoService = new IngressoService(context);

        var filme = new Filme
        {
            IdTMDB = 1,
            Titulo = "Filme Teste",
            Sinopse = "Sinopse teste",
            DuracaoMinutos = 120,
            Genero = "Drama"
        };

        var sala = new Sala
        {
            Nome = "Sala Teste",
            CapacidadeTotal = 100
        };

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        await context.SaveChangesAsync();

        var sessao = new Sessao
        {
            FilmeId = filme.Id,
            SalaId = sala.Id,
            HorarioInicio = DateTime.Now.AddHours(2),
            HorarioFim = DateTime.Now.AddHours(4),
            PrecoBase = 30.00m,
            Status = "Ativa"
        };

        context.Sessoes.Add(sessao);
        await context.SaveChangesAsync();

        // Act
        var ingressoInteira = await ingressoService.ComprarAsync(sessao.Id, "A1", 30.00m, "Inteira");
        var ingressoMeia = await ingressoService.ComprarAsync(sessao.Id, "A2", 15.00m, "Meia");

        // Assert
        Assert.Equal(30.00m, ingressoInteira.Preco);
        Assert.Equal(15.00m, ingressoMeia.Preco);
        Assert.Equal("A1", ingressoInteira.LugarMarcado);
        Assert.Equal("A2", ingressoMeia.LugarMarcado);
    }

    [Fact]
    public async Task DeveCancelarIngressoCorretamente()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var ingressoService = new IngressoService(context);

        var filme = new Filme
        {
            IdTMDB = 1,
            Titulo = "Filme Teste",
            Sinopse = "Sinopse teste",
            DuracaoMinutos = 120,
            Genero = "Comédia"
        };

        var sala = new Sala
        {
            Nome = "Sala Teste",
            CapacidadeTotal = 100
        };

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        await context.SaveChangesAsync();

        var sessao = new Sessao
        {
            FilmeId = filme.Id,
            SalaId = sala.Id,
            HorarioInicio = DateTime.Now.AddHours(2),
            HorarioFim = DateTime.Now.AddHours(4),
            PrecoBase = 25.00m,
            Status = "Ativa"
        };

        context.Sessoes.Add(sessao);
        await context.SaveChangesAsync();

        // Act
        var ingresso = await ingressoService.ComprarAsync(sessao.Id, "A1", 25.00m);
        var cancelado = await ingressoService.CancelarAsync(ingresso.Id);

        // Assert
        Assert.True(cancelado);
        var ingressoCancelado = await context.Ingressos.FindAsync(ingresso.Id);
        Assert.Equal("Cancelado", ingressoCancelado?.StatusIngresso);
    }
}
