namespace ControleAnaliseDesembolso.Domain.Entitys;

// Catálogo dinâmico dos campos "não-validação" da FPD que a macro de
// conferência confere item a item (ex.: "CNPJ Agente Promotor", "Programa").
// Mesmo padrão de Validacao/CAD_TB003_VALIDACAO: nunca apaga uma linha
// (desativa), então contratos antigos continuam com o histórico correto
// mesmo depois que um campo sai de uso.
public class CampoConferencia
{
    public int CoCampo { get; set; }

    // Chave estável usada pelo código (ConferenciaCamposService) pra achar a
    // regra de checagem correspondente — NÃO é o texto exibido. Ex.: "CnpjAgentePromotor".
    public string Chave { get; set; } = string.Empty;

    public string DeCampo { get; set; } = string.Empty;

    public DateTime DtCriacao { get; set; }

    public bool Desativado { get; set; }

    public ICollection<ConferenciaControleDesembolso> Conferencias { get; set; }
        = new List<ConferenciaControleDesembolso>();
}
