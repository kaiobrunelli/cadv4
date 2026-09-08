using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;
using RedeCaixaUtilitario.Domain.Model;

namespace ControleAnaliseDesembolso.Interface
{
    public interface IControleAnaliseDesembolsoService
    {
        Task<string?> ObterREsponsavelDesembolso(int coFpd, CancellationToken cancellationToken = default);

        Task CriarFichaPedidoDesembolso(PedidoDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task ReenviarFichaPedidoDesembolso(int coFpd, PedidoDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task AdicionarComentario(ValidacaoDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task EditarComentario(EditarComentarioRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task RemoverComentario(int coRegistroValidacao, string matriculaSolicitante, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task ValidarDesembolso(int coControleDesembolso, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task ValidarTodosPendentes(PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task<List<DesembolsoResponse>> ObterTodosDesembolsos(CancellationToken cancellationToken = default);
        Task VincularResponsavel(int coControleDesembolso, string? matriculaResponsavel, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task<List<ValidacaoTemplateResponse>> ObterValidacoesTemplate(CancellationToken cancellationToken = default);
        Task<List<ComentarioValidacaoResponse>> ObterComentarios(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task<DesembolsoDetalheResponse> ObterDetalheDesembolso(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task AprovarDesembolso(int coControleDesembolso, AprovarDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task BaixarDRP(int coControleDesembolso, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task RejeitarDesembolso(int coControleDesembolso, RejeitarDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task CancelarDesembolso(int coControleDesembolso, CancelarDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task AtualizarMensagemCefga(int coControleDesembolso, AtualizarMensagemCefgaRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task<List<ConferenciaCampoResponse>> ExecutarConferenciaCampos(int coControleDesembolso, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
        Task<List<RegistroDrpResponse>> ObterRegistrosDrp(CancellationToken cancellationToken = default);
        Task BaixarDrpEmLote(BaixarDrpRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default);
    }
}
