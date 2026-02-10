using Cineflow.Data;
using Cineflow.Models;
using Cineflow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessoesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ISessaoService _sessaoService;

    public SessoesController(AppDbContext db, ISessaoService sessaoService)
    {
        _db = db;
        _sessaoService = sessaoService;
    }

    //GET /api/sessoes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var sessoes = await _sessaoService.GetAllAsync();
            return Ok(sessoes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar sessões.", error = ex.Message });
        }
    }

    //GET /api/sessoes/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var sessao = await _sessaoService.GetByIdAsync(id);

            if (sessao is null)
                return NotFound(new { message = "Sessão não encontrada." });

            return Ok(sessao);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar sessão.", error = ex.Message });
        }
    }

    //POST /api/sessoes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Sessao sessao)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var novaSessao = await _sessaoService.CreateAsync(sessao);
            return CreatedAtAction(nameof(GetById), new { id = novaSessao.Id }, novaSessao);
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
            return StatusCode(500, new { message = "Erro ao criar sessão.", error = ex.Message });
        }
    }

    //PUT /api/sessoes/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Sessao sessao)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        sessao.Id = id;

        try
        {
            var sucesso = await _sessaoService.UpdateAsync(id, sessao);

            if (!sucesso)
                return NotFound(new { message = "Sessão não encontrada." });

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
            return StatusCode(500, new { message = "Erro ao atualizar sessão.", error = ex.Message });
        }
    }

    //DELETE /api/sessoes/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var sucesso = await _sessaoService.DeleteAsync(id);

            if (!sucesso)
                return NotFound(new { message = "Sessão não encontrada." });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao deletar sessão.", error = ex.Message });
        }
    }

    //GET /api/sessoes/cartaz
    [HttpGet("cartaz")]
    public async Task<IActionResult> GetCartaz([FromQuery] int dias = 7)
    {
        try
        {
            if (dias <= 0 || dias > 30)
                return BadRequest(new { message = "O parâmetro 'dias' deve estar entre 1 e 30." });

            var sessoes = await _sessaoService.GetSessoesProximosDiasAsync(dias);
            return Ok(sessoes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar cartaz.", error = ex.Message });
        }
    }
}