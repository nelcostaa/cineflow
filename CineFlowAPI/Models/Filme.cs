public class Filme
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public int DuracaoMinutos { get; set; }
    public string Genero { get; set; }

    public Filme(int id, string titulo, int duracaoMinutos, string genero)
    {
        Id = id;
        Titulo = titulo;
        DuracaoMinutos = duracaoMinutos;
        Genero = genero;
    }
}