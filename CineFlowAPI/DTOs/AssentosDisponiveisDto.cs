namespace Cineflow.Dtos;

public class AssentosDisponiveisDto
{
    public int SessaoId { get; set; }
    public int CapacidadeTotal { get; set; }
    public int IngressosVendidos { get; set; }
    public int LugaresDisponiveis { get; set; }
    public List<string> AssentosOcupados { get; set; } = new();
    public decimal PrecoInteira { get; set; }
    public decimal PrecoMeia { get; set; }
}
