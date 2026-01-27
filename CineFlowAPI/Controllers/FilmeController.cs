using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FilmesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("ok");
}
