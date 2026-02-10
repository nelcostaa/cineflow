using System.Net.Mail;
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

    //GET /api/filmes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var filmes = await _filmeService.GetAllAsync();
            return Ok(filmes);
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro ao buscar filmes.");
        }
    }
    //GET /api/filmes/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var filme = await _filmeService.GetByIdAsync(id);

        if (filme is null)
            return NotFound(new { message = "Filme não encontrado." });

        return Ok(filme);
    }


    /*TODO:
    POST /api/filmes
    PUT /api/filmes/{id}
    DELETE /api/filmes/{id}
    */
}
