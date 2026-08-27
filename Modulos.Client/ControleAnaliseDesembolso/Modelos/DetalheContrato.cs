namespace ControleAnaliseDesembolso.Modelos;

public class DetalheContrato
{
    public string Id { get; set; } = "";
    public string Gigov { get; set; } = "";
    public string NumeroContrato { get; set; } = "";
    public DadosMutuario Mutuario { get; set; } = new();
    public DadosAf Af { get; set; } = new();
    public DadosFinanciamento Financiamento { get; set; } = new();

    public string AgenteFinanceiro { get; set; } = "";
    public string AgentePromotor   { get; set; } = "";
    public bool   PrimeiroDesembolso { get; set; }
    public string TipoDesembolso     { get; set; } = "normal";
}

public class DadosMutuario
{
    public string Nome { get; set; } = "";
    public string Tipo { get; set; } = "";
}

public class DadosAf
{
    public string Nome { get; set; } = "";
    public string Sigla { get; set; } = "";
}

public class DadosFinanciamento
{
    public string Programa { get; set; } = "";
    public decimal ValorInvestimento { get; set; }
    public decimal ValorFinanciamento { get; set; }
    public string NumeroDesembolso { get; set; } = "";
    public string Fase { get; set; } = "";
    public decimal PercentualObra { get; set; }
    public string Amortizacao { get; set; } = "";
    public decimal PercentualContrapartida { get; set; }

    public decimal Ve               { get; set; }
    public decimal ParticipacaoFgts { get; set; }
    public decimal Contrapartida    { get; set; }

    public decimal? PercFgts => Ve > 0 ? Math.Round(ParticipacaoFgts / Ve * 100, 3) : null;
    public decimal? PercGlobal => Ve > 0 ? Math.Round((ParticipacaoFgts + Contrapartida) / Ve * 100, 3) : null;
}
