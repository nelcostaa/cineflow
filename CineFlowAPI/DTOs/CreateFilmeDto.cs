using System.ComponentModel.DataAnnotations;

namespace Cineflow.Dtos;

public class CreateFilmeDto
{
    [Required(ErrorMessage = "O IdTMDB é obrigatório.")]
    public int IdTMDB { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres.")]
    public string Titulo { get; set; } = null!;

    [StringLength(200)]
    public string? TituloOriginal { get; set; }

    [StringLength(10)]
    public string? IdiomaOriginal { get; set; }

    public DateTime? DataLancamento { get; set; }

    [StringLength(2000)]
    public string? Sinopse { get; set; }

    [Range(1, 600, ErrorMessage = "A duração deve estar entre 1 e 600 minutos.")]
    public int? DuracaoMinutos { get; set; }

    [StringLength(100)]
    public string? Genero { get; set; }

    [StringLength(500)]
    public string? PosterPath { get; set; }

    [StringLength(500)]
    public string? BackdropPath { get; set; }

    [Range(0, 10)]
    public decimal? VoteAverage { get; set; }

    [Range(0, int.MaxValue)]
    public int? VoteCount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Popularity { get; set; }

    public bool Adult { get; set; }
    public bool Video { get; set; }
}
