namespace ControleAnaliseDesembolso.Modelos;

public class PreenchimentoFpd
{
    public string    Solicitante  { get; set; } = "";
    public string    Gigov        { get; set; } = "";
    public string    Gestor       { get; set; } = "";
    public string    IdFpd        { get; set; } = "";
    public string    NumeroFpd    { get; set; } = "";
    public int       NuDesembolso { get; set; }
    public string    ContratoAf   { get; set; } = "";
    public string    ContratoAo   { get; set; } = "";
    public string    ContratoAoDv { get; set; } = "";
    public DateTime? DataSolicitado { get; set; } = DateTime.Today;

    // Somente leitura: combina Contrato AO + DV pra exibição; "Não informado" até
    // a CEFGA validar (ver ValidarDesembolso em ControleAnaliseDesembolsoService).
    public string ContratoAoExibicao =>
        string.IsNullOrWhiteSpace(ContratoAo) ? "Não informado"
        : string.IsNullOrWhiteSpace(ContratoAoDv) ? ContratoAo
        : $"{ContratoAo}-{ContratoAoDv}";

    public string? OpcaoExclusiva { get; set; }
    public bool PrimeiroDesembolso => OpcaoExclusiva == "primeiro";
    public bool UltimoDesembolsoSelecionado => OpcaoExclusiva == "ultimo";
    public bool Recorrente => OpcaoExclusiva == "recorrente";

    // Só se aplicam quando Programa == Pro_Transporte
    public string? TemCarroceria { get; set; }
    public string? VeiculoPossuiAdesivos { get; set; }

    public DateTime? DataInicioObra { get; set; }

    // Só se aplica quando PrimeiroDesembolso == true
    public string? DestinacaoColetaResiduosSolidos { get; set; }

    public string AgenteFinanceiro     { get; set; } = "";
    public string CnpjAgenteFinanceiro { get; set; } = "";
    public string Tomador              { get; set; } = "";
    public string CnpjTomador          { get; set; } = "";
    public string AgenteTecnico        { get; set; } = "";
    public string CnpjAgenteTecnico    { get; set; } = "";
    public string AgentePromotor       { get; set; } = "";
    public string CnpjAgentePromotor   { get; set; } = "";
    public string Programa             { get; set; } = "";

    public string? Funcionalidade      { get; set; }
    public string? Conclusao           { get; set; }
    public string? PromotorAdimplente  { get; set; }
    public string? RetornoParcial      { get; set; }
    public string? PlacaLocal          { get; set; }
    public string? LicencaInstalacao   { get; set; }
    public string? LicencaOperacao     { get; set; }
    public string? Excepcionalizacao   { get; set; }
    public string? CpAlterada          { get; set; }
    public string? CndValido           { get; set; }
    public string? CrpValido           { get; set; }
    public string  Mensagem            { get; set; } = "";

    public bool Sanepar { get; set; }

    public DateTime? DataEmissaoEng        { get; set; }
    public string    SituacaoObra          { get; set; } = "NORMAL";
    public DateTime? DataEmissaoSocioAmb   { get; set; }
    public bool      Nsa                   { get; set; }
    public decimal?  PercObra              { get; set; }
    public string TipoDesembolso => OpcaoExclusiva == "adiantamento" ? "adiantamento" : "normal";

    public string    InssObs     { get; set; } = "";

    public decimal?  SolicitadoVi     { get; set; }
    public decimal?  GlosadoVi        { get; set; }
    public decimal?  AceitoVi         { get; set; }
    public decimal?  ParticipacaoFgts { get; set; }
    public decimal?  Contrapartida    { get; set; }
    public decimal?  Ve               { get; set; }
    public decimal?  CpAtual          { get; set; }
    public decimal?  Desembolsado     { get; set; }
    public decimal?  Integralizado    { get; set; }

    // Somente leitura: sempre VE - Desembolsado (o que ainda falta desembolsar
    // do total do empréstimo).
    public decimal? SaldoDesembolsar => Ve is null && Desembolsado is null
        ? null : (Ve ?? 0) - (Desembolsado ?? 0);

    // Somente leitura: sempre CP Atual - (Contrapartida desta parcela + Integralizado
    // histórico) — Integralizado é só o que já foi integralizado ANTES desta
    // parcela (informado pela GIGOV); Contrapartida é o valor desta parcela que
    // está sendo adicionado agora, então os dois precisam somar aqui.
    public decimal? SaldoIntegralizar => CpAtual is null && Integralizado is null && Contrapartida is null
        ? null : (CpAtual ?? 0) - ((Contrapartida ?? 0) + (Integralizado ?? 0));

    // Quanto do VE já foi (ou está sendo, com esse pedido) desembolsado —
    // comparado com PercObra pra ver se o pedido não está pedindo mais do
    // que já foi fisicamente executado na obra.
    public decimal? PercExecucaoFinanceira => Ve is > 0
        ? Math.Round(((Desembolsado ?? 0) + (ParticipacaoFgts ?? 0)) / Ve.Value * 100, 3) : null;

    // Quanto da contrapartida total do contrato (CpAtual) já foi integralizado,
    // somando a Contrapartida desta parcela ao histórico (Integralizado) — mesmo
    // raciocínio do SaldoIntegralizar acima.
    public decimal? PercContrapartidaIntegralizada => CpAtual is > 0
        ? Math.Round(((Contrapartida ?? 0) + (Integralizado ?? 0)) / CpAtual.Value * 100, 3) : null;
}
