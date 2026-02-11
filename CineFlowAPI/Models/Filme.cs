namespace Cineflow.Models;

public class Filme
{
    public int Id { get; set; }
    public int IdTMDB { get; set; }

    public string Titulo { get; set; } = null!;
    public string? TituloOriginal { get; set; }
    public string? IdiomaOriginal { get; set; }

    public DateTime? DataLancamento { get; set; }
    public string? Sinopse { get; set; }

    public int? DuracaoMinutos { get; set; }
    public string? Genero { get; set; }
    public string? ClassificacaoIndicativa { get; set; }

    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }

    public decimal? VoteAverage { get; set; }
    public int? VoteCount { get; set; }
    public decimal? Popularity { get; set; }

    public bool Adult { get; set; }
    public bool Video { get; set; }

    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public List<Sessao> Sessoes { get; set; } = new();
}
