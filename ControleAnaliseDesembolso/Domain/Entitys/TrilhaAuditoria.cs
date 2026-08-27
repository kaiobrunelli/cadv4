namespace ControleAnaliseDesembolso.Domain.Entitys;

public class TrilhaAuditoria
{
    public int CoTrilhaAuditoria { get; set; }

    public required string Usuario { get; set; }

    public required string EnderecoLogicoSolicitante { get; set; }

    public DateTime DataSolicitacao { get; set; }

    public required string Evento { get; set; }

    public required string DescEvento { get; set; }

    public string? Resposta { get; set; }
}
