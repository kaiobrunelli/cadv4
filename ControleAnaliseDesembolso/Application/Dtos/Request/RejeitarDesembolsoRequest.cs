namespace ControleAnaliseDesembolso.Application.Dtos.Request
{
    public class RejeitarDesembolsoRequest
    {
        public string UsuarioNome { get; set; } = string.Empty;
        public string CodigoCoordenacao { get; set; } = string.Empty;
        public string Justificativa { get; set; } = string.Empty;
    }
}
