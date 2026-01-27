public class Ingresso
{
    public int Id { get; set; }
    public int SessaoId { get; set; }
    // public string ClienteNome { get; set; }
    public string LugarMarcado { get; set; }
    public DateTime DataCompra { get; set; }

    //SessaoId -> FK;
    
    public Ingresso(int id, int sessaoId, string lugarMarcado, DateTime dataCompra)
    {
        Id = id;
        SessaoId = sessaoId;
        // ClienteNome = clienteNome;
        LugarMarcado = lugarMarcado;
        DataCompra = dataCompra;
    }


}