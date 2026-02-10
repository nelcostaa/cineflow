using Cineflow.Data;
using Cineflow.Models;
using Cineflow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ISalaService _salaService;
    private readonly ISessaoService _sessaoService;
    private readonly IFilmeService _filmeService;

    public SeedController(AppDbContext db, ISalaService salaService, ISessaoService sessaoService, IFilmeService filmeService)
    {
        _db = db;
        _salaService = salaService;
        _sessaoService = sessaoService;
        _filmeService = filmeService;
    }

    //POST /api/seed/salas
    [HttpPost("salas")]
    public async Task<IActionResult> CriarSalas()
    {
        try
        {
            var salas = new List<Sala>
            {
                new Sala { Nome = "Sala 1 - Premium", CapacidadeTotal = 150 },
                new Sala { Nome = "Sala 2 - VIP", CapacidadeTotal = 80 },
                new Sala { Nome = "Sala 3 - Standard", CapacidadeTotal = 200 },
                new Sala { Nome = "Sala 4 - IMAX", CapacidadeTotal = 300 },
                new Sala { Nome = "Sala 5 - 3D", CapacidadeTotal = 120 },
                new Sala { Nome = "Sala 6 - Mini", CapacidadeTotal = 50 }
            };

            var salasCriadas = new List<Sala>();

            foreach (var sala in salas)
            {
                try
                {
                    var novaSala = await _salaService.CreateAsync(sala);
                    salasCriadas.Add(novaSala);
                }
                catch (InvalidOperationException)
                {
                    // Sala já existe, ignora
                    var existente = await _db.Salas.FirstOrDefaultAsync(s => s.Nome == sala.Nome);
                    if (existente != null)
                        salasCriadas.Add(existente);
                }
            }

            return Ok(new
            {
                message = $"{salasCriadas.Count} salas disponíveis.",
                salas = salasCriadas
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar salas.", error = ex.Message });
        }
    }

    //POST /api/seed/sessoes
    [HttpPost("sessoes")]
    public async Task<IActionResult> CriarSessoes([FromQuery] int diasFuturos = 7)
    {
        try
        {
            var filmes = await _filmeService.GetAllAsync();
            var salas = await _salaService.GetAllAsync();

            if (filmes.Count == 0)
                return BadRequest(new { message = "Nenhum filme cadastrado. Importe filmes primeiro usando POST /api/filmes/importar-now-playing" });

            if (salas.Count == 0)
                return BadRequest(new { message = "Nenhuma sala cadastrada. Crie salas primeiro usando POST /api/seed/salas" });

            var sessoesCriadas = new List<Sessao>();
            var horarios = new[] { 14, 16, 18, 20, 22 }; // Horários das sessões

            var dataInicial = DateTime.Today;
            var dataFinal = dataInicial.AddDays(diasFuturos);

            for (var data = dataInicial; data < dataFinal; data = data.AddDays(1))
            {
                var filmesSorteados = filmes.OrderBy(x => Guid.NewGuid()).Take(Math.Min(6, filmes.Count)).ToList();

                for (int i = 0; i < filmesSorteados.Count && i < salas.Count; i++)
                {
                    var filme = filmesSorteados[i];
                    var sala = salas[i];

                    foreach (var hora in horarios)
                    {
                        var horarioInicio = data.AddHours(hora);
                        var duracaoMinutos = filme.DuracaoMinutos ?? 120; // Duração padrão 2h
                        var horarioFim = horarioInicio.AddMinutes(duracaoMinutos + 30); // +30min para limpeza

                        // Verifica conflito
                        var temConflito = await _sessaoService.HasConflitoAsync(sala.Id, horarioInicio, horarioFim);
                        if (temConflito) continue;

                        var sessao = new Sessao
                        {
                            FilmeId = filme.Id,
                            SalaId = sala.Id,
                            HorarioInicio = horarioInicio,
                            HorarioFim = horarioFim
                        };

                        try
                        {
                            var novaSessao = await _sessaoService.CreateAsync(sessao);
                            sessoesCriadas.Add(novaSessao);
                        }
                        catch
                        {
                            // Ignora conflitos
                        }
                    }
                }
            }

            return Ok(new
            {
                message = $"{sessoesCriadas.Count} sessões criadas com sucesso.",
                sessoes = sessoesCriadas.Select(s => new
                {
                    s.Id,
                    s.FilmeId,
                    Filme = s.Filme?.Titulo,
                    s.SalaId,
                    Sala = s.Sala?.Nome,
                    s.HorarioInicio,
                    s.HorarioFim
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar sessões.", error = ex.Message });
        }
    }

    //POST /api/seed/tudo
    [HttpPost("tudo")]
    public async Task<IActionResult> CriarTudo([FromQuery] int paginasFilmes = 2, [FromQuery] int diasSessoes = 7)
    {
        var resultado = new
        {
            filmes = 0,
            salas = 0,
            sessoes = 0,
            erros = new List<string>()
        };

        try
        {
            // 1. Criar salas
            var resultSalas = await CriarSalas();
            if (resultSalas is OkObjectResult okSalas)
            {
                dynamic? dados = ((OkObjectResult)resultSalas).Value;
                resultado = new
                {
                    filmes = resultado.filmes,
                    salas = dados?.salas?.Count ?? 0,
                    sessoes = resultado.sessoes,
                    erros = resultado.erros
                };
            }

            // 2. Importar filmes (precisa chamar o controller de filmes)
            // Nota: Isso precisa ser feito manualmente ou criar um método auxiliar

            // 3. Criar sessões
            var resultSessoes = await CriarSessoes(diasSessoes);
            if (resultSessoes is OkObjectResult okSessoes)
            {
                dynamic? dados = ((OkObjectResult)resultSessoes).Value;
                resultado = new
                {
                    filmes = resultado.filmes,
                    salas = resultado.salas,
                    sessoes = dados?.sessoes?.Count() ?? 0,
                    erros = resultado.erros
                };
            }

            return Ok(new
            {
                message = "Seed parcial concluído. Execute POST /api/filmes/importar-now-playing para importar filmes.",
                resultado
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao executar seed completo.", error = ex.Message });
        }
    }

    //DELETE /api/seed/limpar
    [HttpDelete("limpar")]
    public async Task<IActionResult> LimparDados()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM Ingressos");
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM Sessoes");
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM Filmes");
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM Salas");

            return Ok(new { message = "Todos os dados foram limpos com sucesso." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao limpar dados.", error = ex.Message });
        }
    }
}
