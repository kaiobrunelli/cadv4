namespace PlataformaNotificacao.Domain
{
    public class EmpregadoAtivo
    {
        public int CodigoEmpregado { get; set; }

        public string? Matricula { get; set; }

        public int? MatriculaDv { get; set; }

        public string? Nome { get; set; }

        public DateTime? DataAdmissao { get; set; }

        public DateTime? DataNascimento { get; set; }

        public int? Cgc { get; set; }

        public int? CodigoFuncao { get; set; }

        public int? TermoLgpd { get; set; }

        public int? CodigoEventual { get; set; }

        public string? Coordenacao { get; set; }

        public DateTime? DataEntrada { get; set; }

        public int? CodigoSituacao { get; set; }
    }
}
