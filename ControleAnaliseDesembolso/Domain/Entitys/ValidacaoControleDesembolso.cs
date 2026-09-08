using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Domain.Entitys;

public class ValidacaoControleDesembolso
{
    // PK substituta: permite mais de uma linha por (CoValidacao,
    // CoControleDesembolso) — necessário pro histórico de
    // ConferenciaLocal/ConferenciaSiapf, que insere uma linha nova a cada
    // execução em vez de sobrescrever. Itens Manual/AutomaticaCampo
    // continuam com uma única linha, mutada em lugar.
    public int CoValidacaoControle { get; set; }

    public int CoValidacao { get; set; }

    public int CoControleDesembolso { get; set; }

    public string? DeValidacao { get; set; }

    public string? CampoVinculado { get; set; }

    public TipoSituacaoValidacao Situacao { get; set; }

    public TipoOrigemValidacao Origem { get; set; }

    // Mensagem de sistema (ex.: motivo de ERRO numa conferência). Comentário
    // com autor/tipo continua na tabela Mensagem à parte.
    public string? Mensagem { get; set; }

    public DateTime DtValidacao { get; set; }

    public Validacao Validacao { get; set; } = null!;

    public ControleDesembolso ControleDesembolso { get; set; } = null!;
}
