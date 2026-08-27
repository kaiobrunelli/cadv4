namespace ControleAnaliseDesembolso.Domain.Entitys;

public class Validacao
{
    public int CoValidacao { get; set; }

    public string? DeValidacao { get; set; }

    public DateTime DtCriacao { get; set; }

    public string? CampoVinculado { get; set; }

    public string? UsuarioExclusao { get; set; }

    public DateTime? DtExclusao { get; set; }

    public bool Desativado { get; set; }

    public ICollection<ValidacaoControleDesembolso> ValidacaoControleDesembolso { get; set; }
        = new List<ValidacaoControleDesembolso>();
}
