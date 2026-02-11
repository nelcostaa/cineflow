namespace Cineflow.Models;

public class Sala
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public int CapacidadeTotal { get; set; }

    public List<Sessao> Sessoes { get; set; } = new();
}
