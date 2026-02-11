namespace Cineflow.Dtos;

public class SessoesPorDiaDto
{
    public string Data { get; set; } = null!;
    public string DiaSemana { get; set; } = null!;
    public List<SessaoResumoDto> Sessoes { get; set; } = new();
}

public class SessaoResumoDto
{
    public int Id { get; set; }
    public string HorarioInicio { get; set; } = null!;
    public string HorarioFim { get; set; } = null!;
    public decimal PrecoBase { get; set; }
    public string Status { get; set; } = null!;

    // Filme
    public int FilmeId { get; set; }
    public string FilmeTitulo { get; set; } = null!;
    public string? FilmePosterPath { get; set; }
    public int? FilmeDuracao { get; set; }
    public string? FilmeClassificacao { get; set; }

    // Sala
    public int SalaId { get; set; }
    public string SalaNome { get; set; } = null!;
    public int SalaCapacidade { get; set; }

    // Ingressos
    public int IngressosVendidos { get; set; }
    public int LugaresDisponiveis { get; set; }
}
