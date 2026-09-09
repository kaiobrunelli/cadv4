namespace RedeCaixaUtilitario.Domain.Model;

// Trilha de auditoria no nível de chamada HTTP/automação (registra "esse
// endpoint foi chamado, com essa resposta"), gerada pela plataforma no
// controller — via IPlataformaOperacionalService.GerarTrilhaDeAuditoria.
// Não confundir com a trilha de auditoria própria do CAD (TrilhaAuditoria,
// CAD_TB000_TRILHA_AUDITORIA), que registra eventos de domínio (ex.: "Aprovou
// o desembolso X") a partir de ControleAnaliseDesembolsoService.RegistrarAuditoria
// — as duas coexistem, em granularidades diferentes.
public class TrilhaDeAuditoria
{
    public string Evento { get; set; } = string.Empty;
    public string DescEvento { get; set; } = string.Empty;
    public string Resposta { get; set; } = string.Empty;
    public string EndpointDisplayName { get; set; } = string.Empty;
    public DateTime DtSolicitacao { get; set; }
}
