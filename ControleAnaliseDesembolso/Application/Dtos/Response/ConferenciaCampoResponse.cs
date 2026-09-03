namespace ControleAnaliseDesembolso.Application.Dtos.Response
{
    public class ConferenciaCampoResponse
    {
        public int CoCampo { get; set; }
        public string DeCampo { get; set; } = string.Empty;
        public string Situacao { get; set; } = string.Empty;
        public string? Mensagem { get; set; }
    }
}
