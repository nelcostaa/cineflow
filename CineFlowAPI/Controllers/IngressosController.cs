using Cineflow.Data;
using Cineflow.Dtos;
using Cineflow.Models;
using Cineflow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Controllers;

[ApiController]
[Route("api")]
public class IngressosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IIngressoService _ingressoService;

    public IngressosController(AppDbContext db, IIngressoService ingressoService)
    {
        _db = db;
        _ingressoService = ingressoService;
    }

    //GET /api/sessoes/{sessaoId}/ingressos
    [HttpGet("sessoes/{sessaoId:int}/ingressos")]
    public async Task<IActionResult> GetBySessao(int sessaoId)
    {
        try
        {
            var ingressos = await _ingressoService.GetBySessaoAsync(sessaoId);
            return Ok(ingressos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar ingressos.", error = ex.Message });
        }
    }

    //GET /api/sessoes/{sessaoId}/assentos-disponiveis
    [HttpGet("sessoes/{sessaoId:int}/assentos-disponiveis")]
    public async Task<IActionResult> GetAssentosDisponiveis(int sessaoId)
    {
        try
        {
            var assentos = await _ingressoService.GetAssentosDisponiveisAsync(sessaoId);
            return Ok(assentos);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar assentos disponíveis.", error = ex.Message });
        }
    }

    //POST /api/sessoes/{sessaoId}/ingressos (compra)
    [HttpPost("sessoes/{sessaoId:int}/ingressos")]
    public async Task<IActionResult> Comprar(int sessaoId, [FromBody] ComprarIngressoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var ingresso = await _ingressoService.ComprarAsync(sessaoId, dto.LugarMarcado, dto.Preco, dto.TipoIngresso);
            return CreatedAtAction(nameof(GetById), new { id = ingresso.Id }, ingresso);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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
            return StatusCode(500, new { message = "Erro ao comprar ingresso.", error = ex.Message });
        }
    }

    //GET /api/ingressos/{id}
    [HttpGet("ingressos/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var ingresso = await _ingressoService.GetByIdAsync(id);

            if (ingresso is null)
                return NotFound(new { message = "Ingresso não encontrado." });

            return Ok(ingresso);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar ingresso.", error = ex.Message });
        }
    }

    //DELETE /api/ingressos/{id} (cancelar)
    [HttpDelete("ingressos/{id:int}")]
    public async Task<IActionResult> Cancelar(int id)
    {
        try
        {
            var sucesso = await _ingressoService.CancelarAsync(id);

            if (!sucesso)
                return NotFound(new { message = "Ingresso não encontrado." });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao cancelar ingresso.", error = ex.Message });
        }
    }
}