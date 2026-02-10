using Cineflow.Data;
using Cineflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalasController : ControllerBase
{
    private readonly AppDbContext _db;
    public SalasController(AppDbContext db) => _db = db;

    /*
    GET /api/salas
    GET /api/salas/{id}
    POST /api/salas
    PUT /api/salas/{id}
    DELETE /api/salas/{id}
    GET /api/salas/{id}/ocupacao?de=&ate= (relatório)
    */

}