public class Sala
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int Capacidade { get; set; }

    public Sala(int id, string nome, int capacidade)
    {
        Id = id;
        Nome = nome;
        Capacidade = capacidade;
    }
}