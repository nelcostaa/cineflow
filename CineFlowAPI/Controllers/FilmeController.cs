using Cineflow.Data;
using Cineflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmesController : ControllerBase
{
    private readonly AppDbContext _db;
    public FilmesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<Filme>>> GetAll()
    {
        var filmes = await _db.Filmes.AsNoTracking().ToListAsync();
        return Ok(filmes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Filme>> GetById(int id)
    {
        var filme = await _db.Filmes.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (filme is null) return NotFound();
        return Ok(filme);
    }

    [HttpPost]
    public async Task<ActionResult<Filme>> Create([FromBody] Filme filme)
    {
        _db.Filmes.Add(filme);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = filme.Id }, filme);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Filme filme)
    {
        if (id != filme.Id) return BadRequest("Id da URL difere do body.");

        var exists = await _db.Filmes.AnyAsync(f => f.Id == id);
        if (!exists) return NotFound();

        _db.Entry(filme).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var filme = await _db.Filmes.FirstOrDefaultAsync(f => f.Id == id);
        if (filme is null) return NotFound();

        _db.Filmes.Remove(filme);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
