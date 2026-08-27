using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Domain.Entitys;

public class ValidacaoControleDesembolso
{
    public int CoValidacao { get; set; }

    public int CoControleDesembolso { get; set; }

    public string? DeValidacao { get; set; }

    public string? CampoVinculado { get; set; }

    public TipoSituacaoValidacao Situacao { get; set; }

    public Validacao Validacao { get; set; } = null!;

    public ControleDesembolso ControleDesembolso { get; set; } = null!;
}
