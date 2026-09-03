namespace ControleAnaliseDesembolso.Modelos;


public class DesembolsoResponseDto
{
    public int CoControleDesembolso { get; set; }
    public string Id { get; set; } = "";
    public string NumId { get; set; } = "";
    public DateTime DtSolicitado { get; set; }
    public string Contrato { get; set; } = "";
    public string Mutuario { get; set; } = "";
    public string Gigov { get; set; } = "";
    public decimal Valor { get; set; }
    public string AgenteFinanceiro { get; set; } = "";
    public string AgentePromotor { get; set; } = "";
    public string MatriculaSolicitante { get; set; } = "";
    public int ValidacoesOk { get; set; }
    public int ValidacoesTotal { get; set; }
    public int Status { get; set; }
    public bool PrimeiroDesembolso { get; set; }
    public bool Adiantamento { get; set; }
    public bool UltimoDesembolso { get; set; }
    public bool Recorrente { get; set; }
    public DateTime PrazoFinal { get; set; }
    public string? ResponsavelAnalise { get; set; }
    public bool DataAgendamento { get; set; }
    public bool Sanepar { get; set; }
    public string? ContratoAo { get; set; }
    public string? ContratoAoDv { get; set; }
    public DateTime? DtConclusao { get; set; }
}

public class ChecklistItemDto
{
    public int CoValidacao { get; set; }
    public string? DeValidacao { get; set; }
    public int Situacao { get; set; }
    public List<ComentarioValidacaoDto> Comentarios { get; set; } = [];
}

public class ComentarioValidacaoDto
{
    public int CoMensagem { get; set; }
    public int CoValidacao { get; set; }
    public string? Texto { get; set; }
    public string TipoMensagem { get; set; } = "";
    public string? MatriculaAutor { get; set; }
    public string? NomeAutor { get; set; }
    public int UnidadeAutor { get; set; }
    public string Sigla { get; set; } = "";
    public DateTime DtCriacao { get; set; }
}

public class ValidacaoTemplateDto
{
    public int CoValidacao { get; set; }
    public string? DeValidacao { get; set; }
}

public class DesembolsoDetalheDto
{
    public int CoControleDesembolso { get; set; }
    public int CoDesembolso { get; set; }
    public int Status { get; set; }
    public DateTime DtSolicitado { get; set; }
    public DateTime DtPrazo { get; set; }
    public DateTime? DtConclusao { get; set; }
    public string? ResponsavelAnalise { get; set; }
    public string? ResponsavelBaixa { get; set; }
    public string CoContratoAf { get; set; } = "";
    public string CoContratoAfDv { get; set; } = "";
    public string? ContratoAo { get; set; }
    public string? ContratoAoDv { get; set; }
    public string CoGigov { get; set; } = "";
    public string MutuarioFinal { get; set; } = "";
    public string CnpjMutuarioFinal { get; set; } = "";
    public string AgenteFinanceiro { get; set; } = "";
    public string AgentePromotor { get; set; } = "";
    public string Programa { get; set; } = "";
    public string TipoDesembolso { get; set; } = "";
    public bool PrimeiroDesembolso { get; set; }
    public bool UltimoDesembolso { get; set; }
    public bool Recorrente { get; set; }
    public decimal PercentualObra { get; set; }
    public decimal ValorEmprestimo { get; set; }
    public decimal SolicitadoVi { get; set; }
    public decimal ParticipacaoFgts { get; set; }
    public decimal Contrapartida { get; set; }

    public string MatriculaSolicitante { get; set; } = "";
    public string MatriculaGestor { get; set; } = "";
    public int NuDesembolso { get; set; }
    public bool? CndValido { get; set; }
    public bool? CrpValido { get; set; }
    public bool CrpNsa { get; set; }
    public string? Mensagem { get; set; }
    public string? MotivoRejeicao { get; set; }
    public string CnpjAf { get; set; } = "";
    public string? AgenteTecnicoOperador { get; set; }
    public string? CnpjAgenteTecnicoOperador { get; set; }
    public string CnpjAgentePromotor { get; set; } = "";
    public DateTime DtEngenharia { get; set; }
    public string? SituacaoObra { get; set; }
    public DateTime? DtSocioAmbiental { get; set; }
    public bool? Concluido { get; set; }
    public decimal GlossadoVi { get; set; }
    public decimal AceitoVi { get; set; }
    public decimal Desembolsado { get; set; }
    public decimal SaldoDesembolsar { get; set; }
    public bool? Excepcionalizado { get; set; }
    public decimal ContrapartidaAtual { get; set; }
    public decimal Integralizado { get; set; }
    public decimal SaldoIntegralizar { get; set; }
    public bool? ContrapartidaAlterada { get; set; }
    public bool? Amortizacao { get; set; }
    public bool? Sanepar { get; set; }
    public bool? RetornoParcial { get; set; }
    public bool? PlacaLocal { get; set; }
    public bool? LicensaInstalacao { get; set; }
    public bool? LicensaOperacao { get; set; }
    public bool? Funcionalidade { get; set; }

    public bool? TemCarroceria { get; set; }
    public bool? VeiculoPossuiAdesivos { get; set; }
    public DateTime? DataInicioObra { get; set; }
    public bool? DestinacaoColetaResiduosSolidos { get; set; }
    public string? MotivoCancelamento { get; set; }

    public string? NumeroDrp { get; set; }
    public string? DvDrp { get; set; }
    public string? SenhaDrp { get; set; }
    public DateTime? DtDrp { get; set; }

    public DateTime? CrfAf { get; set; }
    public DateTime? CrfTomador { get; set; }
    public DateTime? CrfAp { get; set; }
    public DateTime? CrfAt { get; set; }

    public string? MensagemCefga { get; set; }
    public DateTime? DtUltimaConferencia { get; set; }
    public List<ConferenciaCampoDto> ConferenciaCampos { get; set; } = [];

    public List<ChecklistItemDto> Checklist { get; set; } = [];
    public List<ComentarioValidacaoDto> ComentariosGerais { get; set; } = [];
}

public class ConferenciaCampoDto
{
    public int CoCampo { get; set; }
    public string DeCampo { get; set; } = "";
    public string Situacao { get; set; } = "";
    public string? Mensagem { get; set; }
}
