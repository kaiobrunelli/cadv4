namespace RedeCaixaUtilitario.Domain.Model;

public class CadastroGeralSiapf
{
    public string Contrato { get; set; } = string.Empty;
    public string ContratoDv { get; set; } = string.Empty;
    public DadosGerais DadosGerais { get; set; } = new();
    public DadosObra DadosObra { get; set; } = new();
    public List<SelecaoEtapaEvolucaoContratual> EtapaEvolucaoContratual { get; set; } = new();
    public DadosCadastraisAcompanhamento DadosAcompanhamento { get; set; } = new();
    public DadosCadastraisCarencia DadosCarencia { get; set; } = new();
    public DadosCadastraisRetorno DadosRetorno { get; set; } = new();
    public DadosComplementares DadosComplementares { get; set; } = new();
}

public class DadosGerais
{
    public string? ContratoAssociado { get; set; }
    public string? ContratoAssociadoDv { get; set; }
    public string? ContratoPassivo { get; set; }
    public string? ContratoPassivoDv { get; set; }
    public string? ContratoVinculado { get; set; }
    public string? ContratoVinculadoDv { get; set; }
    public string? Tomador { get; set; }
    public string? TomadorDv { get; set; }
    public string? ControleOp { get; set; }
    public string? NaturezaOperacao { get; set; }
    public string? CoMutuarioFinal { get; set; }
    public string? DeMutuarioFinal { get; set; }
    public string? TipoMutuario { get; set; }
    public string? Ugc { get; set; }
    public string? UgcDv { get; set; }
    public string? CoUnidadeMovimento { get; set; }
    public string? CoUnidadeMovimentoDv { get; set; }
    public string? DeUnidadeMovimento { get; set; }
    public string? CoAgentePromotorOuParceiro { get; set; }
    public string? DeAgentePromotorOuParceiro { get; set; }
    public string? CoOrigemRecursos { get; set; }
    public string? DeOrigemRecursos { get; set; }
    public string? CoGerenciaFilial { get; set; }
    public string? CoGerenciaFilialDv { get; set; }
    public string? DeGerenciaFilial { get; set; }
    public string? CoSr { get; set; }
    public string? DeSr { get; set; }
    public string? AnoLimGlob { get; set; }
    public string? CoObjetivo { get; set; }
    public string? DeObjetivo { get; set; }
    public string? DescProjeto { get; set; }
    public DateTime DtAssinatura { get; set; }
    public int DiaEleito { get; set; }
    public double FatSAM { get; set; }
    public double FatConv { get; set; }
    public string? VeHistorico { get; set; }
    public string? CoPadraoMonetario { get; set; }
    public string? DePadraoMonetario { get; set; }
    public int CoSituacaoCobranca { get; set; }
    public string? DeSituacaoCobranca { get; set; }
    public int CoSituacaoContrato { get; set; }
    public string? DeSituacaoContrato { get; set; }
    public string? InstrumentoFormalDeAlteracao { get; set; }
    public DateTime DtInicEEC { get; set; }
    public string? HistoricoAlteracaoCont { get; set; }
    public string? MotivoAlteracao { get; set; }
    public string? TipoGarantia { get; set; }
    public string? Garantidor { get; set; }
    public string? IndiceRiscoDeCredito { get; set; }
    public string? CorreioBacen { get; set; }
    public string? OfStn { get; set; }
    public string? ModBacen { get; set; }
}

public class ConsultaConstrutora
{
    public string? CoConstrutora { get; set; }
    public string? CoConstrutoraDv { get; set; }
    public string? DeConstrutora { get; set; }
    public DateTime DtConceitoConstrutora { get; set; }
}

public class DadosObra
{
    public DateTime DtInicioObra { get; set; }
    public DateTime DtTerminoObra { get; set; }
    public DateTime DtInauguracao { get; set; }
    public DateTime DtAutorCaixa { get; set; }
    public string? LicitacaoConcluida { get; set; }
    public List<RegistroDeObra> RegistrosDeObra { get; set; } = new();
}

public class RegistroDeObra
{
    public DateTime AnoMes { get; set; }
    public double Previsao { get; set; }
    public double Real { get; set; }
    public string? Situacao { get; set; }
    public string? Justificativa { get; set; }
    public string? AdAt { get; set; }
}

public class SelecaoEtapaEvolucaoContratual
{
    public int CoEEC { get; set; }
    public DateTime DtInicio { get; set; }
    public string? MotivoDaCriacao { get; set; }
    public string? Situacao { get; set; }
}

public class DadosCadastraisAcompanhamento
{
    public DateTime Competencia { get; set; }
    public double SaldoDevedor { get; set; }
    public double SaldoCalculaAM { get; set; }
    public double SaldoDevedorENC { get; set; }
    public string? ValTxAdmNumero { get; set; }
    public double TaxaDeJuros { get; set; }
    public int Prazo { get; set; }
    public int NumeroPrestacao { get; set; }
    public double ValorPrestacao { get; set; }
    public string? RazaoRecorrencia { get; set; }
    public string? SistemaDeAmortizacao { get; set; }
    public string? PlanoDeReajustamento { get; set; }
    public string? PeriodicidadeCalculo { get; set; }

    public DateTime? UltimoReajusteSaldoDevedor { get; set; }
    public DateTime? UltimoReajustePrestacao { get; set; }
    public DateTime? UltimaVariacaoTaxaJuros { get; set; }
    public DateTime? UltimoCalculo { get; set; }
}

public class DadosCadastraisCarencia
{
    public DateTime? DtTermino { get; set; }
    public string? CoPeriodicidadeReajusteSaldo { get; set; }
    public string? DePeriodicidadeReajusteSaldo { get; set; }
    public string? CoEpocaReajusteSaldo { get; set; }
    public string? DeEpocaReajusteSaldo { get; set; }
    public string? CoRegAtuaCapCop { get; set; }
    public string? DeRegAtuaCapCop { get; set; }
    public string? CoPeriodicidadeCalculo { get; set; }
    public string? DePeriodicidadeCalculo { get; set; }
    public string? CoEpocaCalculo { get; set; }
    public string? DeEpocaCalculo { get; set; }
    public double TxDeJuros { get; set; }
    public double Impontualidade { get; set; }
    public string? CoJurosRegime { get; set; }
    public string? DeJurosRegime { get; set; }
    public string? CoSeguroRegime { get; set; }
    public string? DeSeguroRegime { get; set; }
    public string? CoPeriodicidadeTxAdministrativa { get; set; }
    public string? DePeriodicidadeTxAdministrativa { get; set; }
    public string? CoEpocaTxAdministrativa { get; set; }
    public string? DeEpocaTxAdministrativa { get; set; }
    public string? CoAtualizaEncargos { get; set; }
    public string? DeAtualizaEncargos { get; set; }
    public string? CoJurosRemuneratorio { get; set; }
    public string? DeJurosRemuneratorio { get; set; }
    public string? CoCritTxAdministracao { get; set; }
    public string? DeCritTxAdministracao { get; set; }
    public double TxAdministracao { get; set; }
    public string? CoTxAcordoBID { get; set; }
    public string? IndiceProrrogTxAdm { get; set; }
}

public class DadosCadastraisRetorno
{
    public int PrazoRetorno { get; set; }
    public string? CoPeriodicidadeReajusteSaldo { get; set; }
    public string? DePeriodicidadeReajusteSaldo { get; set; }
    public string? CoEpocaReajusteSaldo { get; set; }
    public string? DeEpocaReajusteSaldo { get; set; }
    public string? CoRegAtuaCapCop { get; set; }
    public string? DeRegAtuaCapCop { get; set; }
    public string? CoPeriodicidadeCalculo { get; set; }
    public string? DePeriodicidadeCalculo { get; set; }
    public string? CoEpocaCalculo { get; set; }
    public string? DeEpocaCalculo { get; set; }
    public double TxDeJuros { get; set; }
    public double Impontualidade { get; set; }

    public string? CoPeriodoReajustePrestacao { get; set; }
    public string? DePeriodoReajustePrestacao { get; set; }
    public string? CoEpocaReajustePrestacao { get; set; }
    public string? DeEpocaReajustePrestacao { get; set; }
    public string? CoSistemaAmortizacao { get; set; }
    public string? DeSistemaAmortizacao { get; set; }
    public string? CoPlanoReajustePrestacao { get; set; }
    public string? DePlanoReajustePrestacao { get; set; }
    public string? CoCategoriaReajustePrestacao { get; set; }
    public string? DeCategoriaReajustePrestacao { get; set; }
    public double CesFeqPrestacao { get; set; }
    public string? CoAtualizaEncargos { get; set; }
    public string? DeAtualizaEncargos { get; set; }
    public string? CoJurosRemuneratorio { get; set; }
    public string? DeJurosRemuneratorio { get; set; }
    public string? CoPeriodicidadeTxAdministrativa { get; set; }
    public string? DePeriodicidadeTxAdministrativa { get; set; }
    public string? CoEpocaTxAdministrativa { get; set; }
    public string? DeEpocaTxAdministrativa { get; set; }
}

public class DadosComplementares
{
    public string? Empreendimento { get; set; }
    public string? EmpreendimentoDv { get; set; }
    public string? Status { get; set; }
    public string? PvVinculado { get; set; }
    public string? CoCadip { get; set; }
    public string? CoRedur { get; set; }
    public string? DeRedur { get; set; }
    public string? IdExterna { get; set; }
    public string? CtaCorrente { get; set; }
    public string? DadosOrcamentariosCta { get; set; }
    public string? DadosOrcamentariosCtaDv { get; set; }
    public string? ResAut { get; set; }
    public string? AutorizStn { get; set; }
    public string? CoProduto { get; set; }
    public string? DadosVendedorCnpj { get; set; }
    public string? CtaVendedor { get; set; }
    public string? CoOrgAssessor { get; set; }
    public string? DeOrgAssessor { get; set; }
    public string? CoBenFinal { get; set; }
    public string? DeBenFinal { get; set; }
    public string? CoCessionario { get; set; }
    public string? CoCessionarioDv { get; set; }
    public string? DeCessionario { get; set; }
    public string? CtaCorrenteNSGD { get; set; }
    public List<ConsultaConstrutora> ConstrConsult { get; set; } = new();
    public List<string> ContaReserva { get; set; } = new();
    public string? CartaConsulta { get; set; }
    public string? TermoHabilitacao { get; set; }
}
