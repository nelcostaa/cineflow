public class Sessao
{
    public int Id { get; set; }
    public int FilmeId { get; set; }
    public int SalaId { get; set; }
    public DateTime HorarioInicio { get; set; }
    public DateTime HorarioFim { get; set; }

    // FilmeId -> FK;
    // SalaId -> FK;

    public Sessao(int id, int filmeId, int salaId, DateTime horarioInicio, DateTime horarioFim)
    {
        Id = id;
        FilmeId = filmeId;
        SalaId = salaId;
        HorarioInicio = horarioInicio;
        HorarioFim = horarioFim;
    }
}