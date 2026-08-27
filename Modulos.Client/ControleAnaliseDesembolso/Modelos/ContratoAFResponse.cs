namespace ControleAnaliseDesembolso.Modelos;


public record ContratoAFResponse
{
    public string CoContratoAF { get; set; } = string.Empty;
    public string CoContratoAFDV { get; set; } = string.Empty;
    public int NuFpd { get; set; }
    public string CoGigov { get; set; } = string.Empty;
    public string MatriculaGestor { get; set; } = string.Empty;

    public string AgenteFinanceiro { get; set; } = string.Empty;
    public string CnpjAf { get; set; } = string.Empty;
    public string MutuarioFinal { get; set; } = string.Empty;
    public string CnpjMutuarioFinal { get; set; } = string.Empty;
    public string? AgenteTecnicoOperador { get; set; }
    public string? CnpjAgenteTecnicoOperador { get; set; }
    public string AgentePromotor { get; set; } = string.Empty;
    public string CnpjAgentePromotor { get; set; } = string.Empty;

    public string Programa { get; set; } = string.Empty;

    public bool RetornoParcial { get; set; }
    public DateTime DtEngenharia { get; set; }
    public string? SituacaoObra { get; set; }
    public DateTime? DtSocioAmbiental { get; set; }
    public decimal PercentualObra { get; set; }

    public decimal ValorEmprestimo { get; set; }
    public decimal Desembolsado { get; set; }

    public decimal ContrapartidaAtual { get; set; }
    public decimal Integralizado { get; set; }
}