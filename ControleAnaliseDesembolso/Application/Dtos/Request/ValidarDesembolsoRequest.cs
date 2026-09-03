namespace ControleAnaliseDesembolso.Application.Dtos.Request
{
    public class ValidarDesembolsoRequest
    {
        public string MatriculaUsuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
