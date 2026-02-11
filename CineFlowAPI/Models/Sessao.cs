namespace Cineflow.Models;

public class Sessao
{
    public int Id { get; set; }

    public int FilmeId { get; set; }
    public Filme? Filme { get; set; } = null!;

    public int SalaId { get; set; }
    public Sala? Sala { get; set; } = null!;

    public DateTime HorarioInicio { get; set; }
    public DateTime HorarioFim { get; set; }

    public decimal PrecoBase { get; set; }
    public string Status { get; set; } = "Ativa";

    public List<Ingresso> Ingressos { get; set; } = new();
}
