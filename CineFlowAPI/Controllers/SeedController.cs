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
    
    private decimal CalcularPrecoBasePorSala(Sala sala)
    {
        var nomeSala = sala.Nome?.ToLower() ?? "";

        if (nomeSala.Contains("premium")) return 45.00m;
        if (nomeSala.Contains("vip")) return 50.00m;
        if (nomeSala.Contains("imax")) return 55.00m;
        if (nomeSala.Contains("3d")) return 40.00m;
        if (nomeSala.Contains("mini")) return 20.00m;
        if (nomeSala.Contains("standard")) return 30.00m;

        // Preço padrão baseado na capacidade se não identificar o tipo
        if (sala.CapacidadeTotal < 80) return 20.00m;
        if (sala.CapacidadeTotal < 150) return 30.00m;
        if (sala.CapacidadeTotal < 250) return 40.00m;
        return 50.00m;
    }

    //POST /api/seed/sala-unica
    [HttpPost("sala-unica")]
    public async Task<IActionResult> CriarSalaUnica()
    {
        try
        {
            var contador = await _db.Salas.CountAsync() + 1;
            var sala = new Sala
            {
                Nome = $"Sala {contador}",
                CapacidadeTotal = 100
            };

            var novaSala = await _salaService.CreateAsync(sala);

            return Ok(new
            {
                message = "Sala criada com sucesso.",
                sala = novaSala
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar sala.", error = ex.Message });
        }
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

    //POST /api/seed/sessao-unica
    [HttpPost("sessao-unica")]
    public async Task<IActionResult> CriarSessaoUnica()
    {
        try
        {
            var filmes = await _filmeService.GetAllAsync();
            var salas = await _salaService.GetAllAsync();

            if (filmes.Count == 0)
                return BadRequest(new { message = "Nenhum filme cadastrado. Importe filmes primeiro." });

            if (salas.Count == 0)
                return BadRequest(new { message = "Nenhuma sala cadastrada. Crie uma sala primeiro." });

            var filme = filmes.OrderBy(x => Guid.NewGuid()).First();
            var sala = salas.OrderBy(x => Guid.NewGuid()).First();
            var horario = 14 + (new Random().Next(0, 5) * 2);
            var horarioInicio = DateTime.Today.AddDays(1).AddHours(horario);
            var duracaoMinutos = filme.DuracaoMinutos ?? 120;
            var horarioFim = horarioInicio.AddMinutes(duracaoMinutos + 30);

            var sessao = new Sessao
            {
                FilmeId = filme.Id,
                SalaId = sala.Id,
                HorarioInicio = horarioInicio,
                HorarioFim = horarioFim,
                PrecoBase = CalcularPrecoBasePorSala(sala),
                Status = "Ativa"
            };

            var novaSessao = await _sessaoService.CreateAsync(sessao);

            return Ok(new
            {
                message = "Sessão criada com sucesso.",
                sessao = new
                {
                    novaSessao.Id,
                    novaSessao.FilmeId,
                    Filme = filme.Titulo,
                    novaSessao.SalaId,
                    Sala = sala.Nome,
                    novaSessao.HorarioInicio,
                    novaSessao.HorarioFim,
                    novaSessao.PrecoBase,
                    novaSessao.Status
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar sessão.", error = ex.Message });
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
            var horarios = new[] { 14, 16, 18, 20, 22 };
            var random = new Random();

            var dataInicial = DateTime.Today;
            var dataFinal = dataInicial.AddDays(diasFuturos);

            for (var data = dataInicial; data < dataFinal; data = data.AddDays(1))
            {
                if (sessoesCriadas.Count >= 12) break;

                var filmesSorteados = filmes.OrderBy(x => Guid.NewGuid()).Take(Math.Min(6, filmes.Count)).ToList();

                for (int i = 0; i < filmesSorteados.Count && i < salas.Count; i++)
                {
                    if (sessoesCriadas.Count >= 12) break;

                    var filme = filmesSorteados[i];
                    var sala = salas[i];

                    foreach (var hora in horarios)
                    {
                        if (sessoesCriadas.Count >= 12) break;

                        var horarioInicio = data.AddHours(hora);
                        var duracaoMinutos = filme.DuracaoMinutos ?? 120;
                        var horarioFim = horarioInicio.AddMinutes(duracaoMinutos + 30);

                        var temConflito = await _sessaoService.HasConflitoAsync(sala.Id, horarioInicio, horarioFim);
                        if (temConflito) continue;

                        var sessao = new Sessao
                        {
                            FilmeId = filme.Id,
                            SalaId = sala.Id,
                            HorarioInicio = horarioInicio,
                            HorarioFim = horarioFim,
                            PrecoBase = CalcularPrecoBasePorSala(sala),
                            Status = "Ativa"
                        };

                        var novaSessao = await _sessaoService.CreateAsync(sessao);
                        sessoesCriadas.Add(novaSessao);

                        var capacidade = sala.CapacidadeTotal;
                        var percentualOcupacao = random.Next(20, 60);
                        var quantidadeIngressos = (int)(capacidade * percentualOcupacao / 100.0);

                        var assentosOcupados = new HashSet<string>();
                        for (int j = 0; j < quantidadeIngressos; j++)
                        {

                            string assento;
                            do
                            {
                                var fileira = (char)('A' + random.Next(0, 8));
                                var numero = random.Next(1, 11);
                                assento = $"{fileira}{numero}";
                            } while (assentosOcupados.Contains(assento));

                            assentosOcupados.Add(assento);

                            var tipoIngresso = random.Next(0, 100) < 40 ? "Meia" : "Inteira";
                            var preco = tipoIngresso == "Meia" ? sessao.PrecoBase / 2 : sessao.PrecoBase;

                            var ingresso = new Ingresso
                            {
                                SessaoId = novaSessao.Id,
                                LugarMarcado = assento,
                                Preco = preco,
                                DataCompra = DateTime.Now.AddDays(-random.Next(0, 3)),
                                StatusIngresso = "Ativo"
                            };

                            _db.Ingressos.Add(ingresso);
                        }
                    }
                }
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = $"{sessoesCriadas.Count} sessões criadas com sucesso (com ingressos parcialmente vendidos).",
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
            var resultSalas = await CriarSalas();
            int salasCount = 0;
            if (resultSalas is OkObjectResult okSalas)
            {
                dynamic? dados = ((OkObjectResult)resultSalas).Value;
                salasCount = dados?.salas?.Count ?? 0;
            }
            var resultSessoes = await CriarSessoes(diasSessoes);
            int sessoesCount = 0;
            if (resultSessoes is OkObjectResult okSessoes)
            {
                dynamic? dados = ((OkObjectResult)resultSessoes).Value;
                sessoesCount = dados?.sessoes?.Count() ?? 0;
            }

            return Ok(new
            {
                message = "Seed parcial concluído. Execute POST /api/filmes/importar-now-playing para importar filmes.",
                resultado = new
                {
                    filmes = resultado.filmes,
                    salas = salasCount,
                    sessoes = sessoesCount,
                    erros = resultado.erros
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao executar seed completo.", error = ex.Message });
        }
    }

    //POST /api/seed/ingresso-unico
    [HttpPost("ingresso-unico")]
    public async Task<IActionResult> CriarIngressoUnico()
    {
        try
        {
            var sessoes = await _db.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                .Where(s => s.Status == "Ativa" && s.HorarioInicio > DateTime.Now)
                .ToListAsync();

            if (sessoes.Count == 0)
                return BadRequest(new { message = "Nenhuma sessão ativa disponível. Crie uma sessão primeiro." });

            var sessao = sessoes.OrderBy(x => Guid.NewGuid()).First();

            var ingressosExistentes = await _db.Ingressos
                .Where(i => i.SessaoId == sessao.Id && i.StatusIngresso == "Ativo")
                .CountAsync();

            if (ingressosExistentes >= sessao.Sala!.CapacidadeTotal)
                return BadRequest(new { message = "A sessão selecionada está lotada. Tente novamente." });

            var lugarMarcado = $"A{ingressosExistentes + 1}";
            var ingresso = new Ingresso
            {
                SessaoId = sessao.Id,
                LugarMarcado = lugarMarcado,
                Preco = sessao.PrecoBase,
                DataCompra = DateTime.Now,
                StatusIngresso = "Ativo"
            };

            _db.Ingressos.Add(ingresso);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Ingresso criado com sucesso.",
                ingresso = new
                {
                    ingresso.Id,
                    ingresso.SessaoId,
                    Filme = sessao.Filme?.Titulo,
                    Sala = sessao.Sala?.Nome,
                    Horario = sessao.HorarioInicio,
                    ingresso.LugarMarcado,
                    ingresso.Preco,
                    ingresso.DataCompra,
                    ingresso.StatusIngresso
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar ingresso.", error = ex.Message });
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
