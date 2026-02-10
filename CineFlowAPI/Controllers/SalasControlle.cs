using Cineflow.Data;
using Cineflow.Models;
using Cineflow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalasController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ISalaService _salaService;

    public SalasController(AppDbContext db, ISalaService salaService)
    {
        _db = db;
        _salaService = salaService;
    }

    //GET /api/salas
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var salas = await _salaService.GetAllAsync();
            return Ok(salas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar salas.", error = ex.Message });
        }
    }

    //GET /api/salas/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var sala = await _salaService.GetByIdAsync(id);

            if (sala is null)
                return NotFound(new { message = "Sala não encontrada." });

            return Ok(sala);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar sala.", error = ex.Message });
        }
    }

    //POST /api/salas
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Sala sala)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var novaSala = await _salaService.CreateAsync(sala);
            return CreatedAtAction(nameof(GetById), new { id = novaSala.Id }, novaSala);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar sala.", error = ex.Message });
        }
    }

    //PUT /api/salas/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Sala sala)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var sucesso = await _salaService.UpdateAsync(id, sala);

            if (!sucesso)
                return NotFound(new { message = "Sala não encontrada." });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao atualizar sala.", error = ex.Message });
        }
    }

    //DELETE /api/salas/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var sucesso = await _salaService.DeleteAsync(id);

            if (!sucesso)
                return NotFound(new { message = "Sala não encontrada." });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao deletar sala.", error = ex.Message });
        }
    }

    //GET /api/salas/{id}/ocupacao?de=&ate= (relatório)
    [HttpGet("{id:int}/ocupacao")]
    public async Task<IActionResult> GetOcupacao(int id, [FromQuery] DateTime? de = null, [FromQuery] DateTime? ate = null)
    {
        try
        {
            var taxaOcupacao = await _salaService.GetTaxaOcupacaoSalaAsync(id, de, ate);

            return Ok(new
            {
                salaId = id,
                periodoInicio = de ?? DateTime.UtcNow.AddDays(-30),
                periodoFim = ate ?? DateTime.UtcNow,
                taxaOcupacaoPercentual = Math.Round(taxaOcupacao, 2)
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao calcular ocupação.", error = ex.Message });
        }
    }
}