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

    public Desembolso Desembolso { get; set; } = null!;

    public ICollection<ValidacaoControleDesembolso> ValidacaoControleDesembolso { get; set; }
        = new List<ValidacaoControleDesembolso>();
}
