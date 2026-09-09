using RedeCaixaUtilitario.Domain.Model;

namespace RedeCaixaUtilitario.Application.Interface;

// Serviço transversal da plataforma (não específico do CAD): identifica o
// usuário autenticado, monta o PedidoDeAutomacao/TrilhaDeAuditoria de cada
// chamada e persiste essa auditoria de nível de endpoint. Implementação real
// fica a cargo da plataforma (RedeCaixa) — ver PlataformaOperacionalServiceTemporario
// pra o que está de pé hoje enquanto isso não chega.
public interface IPlataformaOperacionalService
{
    Usuario IdentificarUsuario();

    PedidoDeAutomacao GerarPedidoDeAutomacao(string area, string endpoint, string senha, bool assincrono);

    TrilhaDeAuditoria GerarTrilhaDeAuditoria(string evento, string descEvento, string respostaInicial, string endpointDisplayName);

    Task SaveAsync();
}
