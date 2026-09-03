using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Domain.Entitys;

// Resultado de uma rodada da macro de conferência pra 1 campo de 1 desembolso.
// DeCampo é uma cópia (snapshot) da descrição do catálogo no momento da
// conferência — mesmo motivo do DeValidacao em ValidacaoControleDesembolso:
// se o catálogo mudar de texto depois, o histórico já gerado não muda.
public class ConferenciaControleDesembolso
{
    public int CoConferencia { get; set; }

    public int CoControleDesembolso { get; set; }

    public int CoCampo { get; set; }

    public string DeCampo { get; set; } = string.Empty;

    public TipoSituacaoConferencia Situacao { get; set; }

    public string? Mensagem { get; set; }

    public DateTime DtConferencia { get; set; }

    public CampoConferencia CampoConferencia { get; set; } = null!;

    public ControleDesembolso ControleDesembolso { get; set; } = null!;
}
