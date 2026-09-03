namespace ControleAnaliseDesembolso.Application.Dtos.Request
{
    public class CancelarDesembolsoRequest
    {
        public string MatriculaUsuario { get; set; } = string.Empty;
        public string UsuarioNome { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
    }
}
