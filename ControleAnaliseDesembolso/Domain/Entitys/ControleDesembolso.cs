using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Domain.Entitys;

public class ControleDesembolso
{
    public int CoControleDesembolso { get; set; }

    public int CoDesembolso { get; set; }

    public string? ResponsavelAnalise { get; set; }

    public string? ResponsavelBaixa { get; set; }

    public string? ResponsavelDesembolso { get; set; }

    public string? Gestor { get; set; }

    public DateTime DtPrazo { get; set; }

    public TipoStatusDesembolso StatusDesembolso { get; set; }

    public DateTime? DtConclusao { get; set; }

    public string? MotivoCancelamento { get; set; }

    // Null = a macro de conferência de campos ainda não rodou pra esse desembolso.
    public DateTime? DtUltimaConferencia { get; set; }

    // Dados da DRP emitida pra esse desembolso — todos nulos até a DRP ser emitida.
    public string? NumeroDrp { get; set; }
    public string? DvDrp { get; set; }
    public string? SenhaDrp { get; set; }
    public DateTime? DtDrp { get; set; }

    // CRF (Certificado de Regularidade do FGTS) de cada parte do contrato —
    // data de validade/verificação, nula até ser conferida.
    public DateTime? CrfAf { get; set; }
    public DateTime? CrfTomador { get; set; }
    public DateTime? CrfAp { get; set; }
    public DateTime? CrfAt { get; set; }

    public Desembolso Desembolso { get; set; } = null!;

    public ICollection<ValidacaoControleDesembolso> ValidacaoControleDesembolso { get; set; }
        = new List<ValidacaoControleDesembolso>();

    public ICollection<ConferenciaControleDesembolso> Conferencias { get; set; }
        = new List<ConferenciaControleDesembolso>();
}
