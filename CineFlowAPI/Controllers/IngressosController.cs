using Cineflow.Data;
using Cineflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cineflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngressosController : ControllerBase
{
    private readonly AppDbContext _db;
    public IngressosController(AppDbContext db) => _db = db;

    /*
    GET /api/sessoes/{sessaoId}/ingressos
    POST /api/sessoes/{sessaoId}/ingressos ✅ compra (lotação + assento)
    GET /api/ingressos/{id}
    DELETE /api/ingressos/{id}
    */
}