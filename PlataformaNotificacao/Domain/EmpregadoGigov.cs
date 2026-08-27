namespace PlataformaNotificacao.Domain
{
    public class EmpregadoGigov
    {
        public int CodigoEmpregado { get; set; }

        public string Matricula { get; set; } = "";

        public string Nome { get; set; } = "";

        public string CodigoGigov { get; set; } = "";

        public bool Ativo { get; set; } = true;
    }
}
