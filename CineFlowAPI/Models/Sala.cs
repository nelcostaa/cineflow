namespace Cineflow.Models;

public class Sala
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public int CapacidadeTotal { get; set; }

    //Verificar a necessidade de salvar essa informação, ou se é melhor calcular a lotação atual a partir dos ingressos vendidos para as sessões.
    public int LotacaoAtual { get; set; } = 0;

    public List<Sessao> Sessoes { get; set; } = new();
}
