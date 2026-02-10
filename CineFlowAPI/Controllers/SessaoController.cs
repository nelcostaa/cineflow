using Cineflow.Data;
using Cineflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessaoController : ControllerBase
{
    private readonly AppDbContext _db;
    public SessaoController(AppDbContext db) => _db = db;

    /*
    GET /api/sessoes
    GET /api/sessoes/{id}
    POST /api/sessoes
    PUT /api/sessoes/{id}
    DELETE /api/sessoes/{id}
    GET /api/cartaz (sessoes próximas, 7 dias)
    */
}