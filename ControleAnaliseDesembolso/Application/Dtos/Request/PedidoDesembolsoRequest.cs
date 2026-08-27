namespace ControleAnaliseDesembolso.Application.Dtos.Request
{
    public class PedidoDesembolsoRequest
    {
        public string MatriculaSolicitante { get; set; } = string.Empty;

        public string CoGigov { get; set; } = string.Empty;
        public string MatriculaGestor { get; set; } = string.Empty;
        public int NuDesembolso { get; set; }
        public string CoContratoAf { get; set; } = string.Empty;
        public string CoContratoAfDv { get; set; } = string.Empty;
        public bool PrimeiroDesembolso { get; set; }
        public string AgenteFinanceiro { get; set; } = string.Empty;
        public string CnpjAf { get; set; } = string.Empty;
        public string MutuarioFinal { get; set; } = string.Empty;
        public string CnpjMutuarioFinal { get; set; } = string.Empty;

        public string? AgenteTecnicoOperador { get; set; }
        public string? CnpjAgenteTecnicoOperador { get; set; }
        public string AgentePromotor { get; set; } = string.Empty;
        public string CnpjAgentePromotor { get; set; } = string.Empty;
        public int Programa { get; set; }
        public bool UltimoDesembolso { get; set; }
        public bool? Funcionalidade { get; set; }
        public bool? Concluido { get; set; }
        public DateTime DtEngenharia { get; set; }
        public int SituacaoObra { get; set; }

        public DateTime? DtSocioAmbiental { get; set; }
        public decimal PercentualObra { get; set; }
        public int TipoDesembolso { get; set; }
        public bool? RetornoParcial { get; set; }
        public bool? PlacaLocal { get; set; }
        public bool? LicensaInstalacao { get; set; }
        public bool? LicensaOperacao { get; set; }
        public bool? CndValido { get; set; }
        public bool? CrpValido { get; set; }
        public decimal SolicitadoVi { get; set; }
        public decimal GlossadoVi { get; set; }
        public decimal AceitoVi { get; set; }

        public decimal ParticipacaoFgts { get; set; }
        public decimal Contrapartida { get; set; }
        public decimal ValorEmprestimo { get; set; }
        public decimal Desembolsado { get; set; }
        public decimal SaldoDesembolsar { get; set; }
        public bool? Excepcionalizado { get; set; }
        public decimal ContrapartidaAtual { get; set; }
        public decimal Integralizado { get; set; }
        public decimal SaldoIntegralizar { get; set; }
        public bool? ContrapartidaAlterada { get; set; }
        public bool? Amortizacao { get; set; }
        public bool? Sanepar { get; set; }
        public string? Mensagem { get; set; }

        public List<ValidacaoDesembolsoRequest> ValidacoesDesembolsoRequest { get; set; } = new();
    }
}
