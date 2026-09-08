using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Domain.Entitys;

public class Validacao
{
    public int CoValidacao { get; set; }

    public string? DeValidacao { get; set; }

    public DateTime DtCriacao { get; set; }

    // Pra itens Manual/AutomaticaCampo, geralmente null. Pra itens
    // ConferenciaLocal/ConferenciaSiapf, é a chave estável (ex.:
    // "AgenteFinanceiro", "SiapfContratoAf") usada em
    // ExecutarConferenciaCamposInterno pra achar a regra de checagem em
    // _regrasConferencia/_regrasConferenciaSiapf.
    public string? CampoVinculado { get; set; }

    public TipoOrigemValidacao Origem { get; set; }

    public string? UsuarioExclusao { get; set; }

    public DateTime? DtExclusao { get; set; }

    public bool Desativado { get; set; }

    public ICollection<ValidacaoControleDesembolso> ValidacaoControleDesembolso { get; set; }
        = new List<ValidacaoControleDesembolso>();
}
