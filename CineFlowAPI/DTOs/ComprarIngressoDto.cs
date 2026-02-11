using System.ComponentModel.DataAnnotations;

namespace Cineflow.Dtos;

public class ComprarIngressoDto
{
    [Required(ErrorMessage = "O lugar marcado é obrigatório.")]
    public string LugarMarcado { get; set; } = null!;

    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    public string TipoIngresso { get; set; } = "Inteira";
}
