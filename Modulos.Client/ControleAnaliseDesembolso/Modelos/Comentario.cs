namespace ControleAnaliseDesembolso.Modelos;

public class Comentario
{
    public int      Id             { get; set; }
    public string   Tipo           { get; set; } = "informativo";
    public string   Texto          { get; set; } = "";
    public string   Autor          { get; set; } = "";
    public string   MatriculaAutor { get; set; } = "";
    public int      UnidadeAutor   { get; set; }
    public DateTime Timestamp      { get; set; } = DateTime.Now;
    public DateTime? EditadoEm     { get; set; }
}

public static class TiposComentario
{
    public static readonly Dictionary<string, ConfigTipo> Configs = new()
    {
        ["justificativa"] = new("#E0F2E7", "#1A7A4A", "#1A7A4A"),
        ["informativo"]   = new("#E5F1FC", "#00437A", "#005CA9"),
        ["parecer"]       = new("#EEF0F3", "#4B5563", "#6B7280"),
    };

    public record ConfigTipo(string CorFundo, string CorTexto, string CorPonto);
}


