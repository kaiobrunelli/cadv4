namespace ControleAnaliseDesembolso.Application.Dtos.Request
{
    public class CancelarDesembolsoRequest
    {
        public string UsuarioNome { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
    }
}
