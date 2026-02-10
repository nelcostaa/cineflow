namespace Cineflow.Models;

public class Ingresso
{
    public int Id { get; set; }

    public int SessaoId { get; set; }
    public Sessao Sessao { get; set; } = null!;

    public string LugarMarcado { get; set; } = null!;
    public decimal Preco { get; set; }
    public DateTime DataCompra { get; set; }
}
