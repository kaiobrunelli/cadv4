using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;

namespace ControleAnaliseDesembolso.Interface
{
    public interface IControleAnaliseDesembolsoService
    {
        Task<string?> ObterREsponsavelDesembolso(int coFpd, CancellationToken cancellationToken = default);

        Task CriarFichaPedidoDesembolso(PedidoDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task ReenviarFichaPedidoDesembolso(int coFpd, PedidoDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task AdicionarComentario(ValidacaoDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task EditarComentario(EditarComentarioRequest request, CancellationToken cancellationToken = default);
        Task RemoverComentario(int coRegistroValidacao, string matriculaSolicitante, CancellationToken cancellationToken = default);
        Task ValidarDesembolso(int coControleDesembolso, ValidarDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task ValidarTodosPendentes(ValidarDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task<List<DesembolsoResponse>> ObterTodosDesembolsos(CancellationToken cancellationToken = default);
        Task VincularResponsavel(int coControleDesembolso, string? matriculaResponsavel, CancellationToken cancellationToken = default);
        Task<List<ValidacaoTemplateResponse>> ObterValidacoesTemplate(CancellationToken cancellationToken = default);
        Task<List<ComentarioValidacaoResponse>> ObterComentarios(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task<DesembolsoDetalheResponse> ObterDetalheDesembolso(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task AprovarDesembolso(int coControleDesembolso, AprovarDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task BaixarDRP(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task RejeitarDesembolso(int coControleDesembolso, RejeitarDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task CancelarDesembolso(int coControleDesembolso, CancelarDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task AtualizarMensagemCefga(int coControleDesembolso, AtualizarMensagemCefgaRequest request, CancellationToken cancellationToken = default);
        Task<List<ConferenciaCampoResponse>> ExecutarConferenciaCampos(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task<List<RegistroDrpResponse>> ObterRegistrosDrp(CancellationToken cancellationToken = default);
        Task BaixarDrpEmLote(BaixarDrpRequest request, CancellationToken cancellationToken = default);
    }
}
