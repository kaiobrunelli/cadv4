using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Domain.Entitys;

namespace PlataformaOperacional.Application.Service.Interface
{
    public interface IAplicacaoService
    {
        #region Controle Análise Desembolso

        Task<List<DesembolsoResponse>> ObterTodosDesembolsos(CancellationToken cancellationToken = default);
        Task<DesembolsoDetalheResponse> ObterDetalheDesembolso(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task<List<ComentarioValidacaoResponse>> ObterComentarios(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task AdicionarComentario(ValidacaoDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task EditarComentario(EditarComentarioRequest request, CancellationToken cancellationToken = default);
        Task RemoverComentario(int coRegistroValidacao, string matriculaSolicitante, CancellationToken cancellationToken = default);
        Task AprovarDesembolso(int coControleDesembolso, AprovarDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task BaixarDRP(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task RejeitarDesembolso(int coControleDesembolso, RejeitarDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task VincularResponsavel(int coControleDesembolso, string? matriculaResponsavel, CancellationToken cancellationToken = default);
        Task ValidarDesembolso(int coControleDesembolso, CancellationToken cancellationToken = default);
        Task ValidarTodosPendentes(CancellationToken cancellationToken = default);
        Task<List<ValidacaoTemplateResponse>> ObterValidacoesTemplate(CancellationToken cancellationToken = default);
        Task<PedidoConsultaContratoAfResponse> SolicitarDadosFPD(PedidoConsultaContratoAfRequest pedido, CancellationToken cancellationToken = default);
        Task CriarFichaPedidoDesembolso(PedidoDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task ReenviarFichaPedidoDesembolso(int coFpd, PedidoDesembolsoRequest request, CancellationToken cancellationToken = default);
        Task<List<RegistroDrpResponse>> ObterRegistrosDrp(CancellationToken cancellationToken = default);
        Task BaixarDrpEmLote(BaixarDrpRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region Empregado

        Task<List<Empregado>> ObterEmpregadosPorCoordenacao(string coordenacao);
        Task<List<string>> ObterCodigosGigovPorMatricula(string matricula);

        #endregion
    }
}
