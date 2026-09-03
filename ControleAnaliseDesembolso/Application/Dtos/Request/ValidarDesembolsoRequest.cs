namespace ControleAnaliseDesembolso.Application.Dtos.Request
{
    // Credenciais de quem clicou em "Validar" — usadas só pra logar no SIAPF
    // durante a consulta ao cadastro geral do contrato (ver ISiapfService),
    // não ficam guardadas em nenhuma tabela.
    public class ValidarDesembolsoRequest
    {
        public string MatriculaUsuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
