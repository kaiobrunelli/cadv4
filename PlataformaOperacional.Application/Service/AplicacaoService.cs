using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Application.Interface;
using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Interface;
using PlataformaOperacional.Application.Service.Interface;

namespace PlataformaOperacional.Application.Service
{
    public class AplicacaoService : IAplicacaoService
    {
        private readonly IControleAnaliseDesembolsoService _controleAnaliseDesembolso;
        private readonly IFichaPedidoDesembolsoService _fichaPedidoDesembolso;
        private readonly IEmpregadoCADService _empregados;

        public AplicacaoService(
            IControleAnaliseDesembolsoService controleAnaliseDesembolso,
            IFichaPedidoDesembolsoService fichaPedidoDesembolso,
            IEmpregadoCADService empregados)
        {
            _controleAnaliseDesembolso = controleAnaliseDesembolso;
            _fichaPedidoDesembolso = fichaPedidoDesembolso;
            _empregados = empregados;
        }

        #region Controle Análise Desembolso

        public async Task<List<DesembolsoResponse>> ObterTodosDesembolsos(CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterTodosDesembolsos(cancellationToken);
        public async Task<DesembolsoDetalheResponse> ObterDetalheDesembolso(int coControleDesembolso, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterDetalheDesembolso(coControleDesembolso, cancellationToken);
        public async Task<List<ComentarioValidacaoResponse>> ObterComentarios(int coControleDesembolso, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterComentarios(coControleDesembolso, cancellationToken);
        public async Task AdicionarComentario(ValidacaoDesembolsoRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.AdicionarComentario(request, cancellationToken);
        public async Task EditarComentario(EditarComentarioRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.EditarComentario(request, cancellationToken);
        public async Task RemoverComentario(int coRegistroValidacao, string matriculaSolicitante, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.RemoverComentario(coRegistroValidacao, matriculaSolicitante, cancellationToken);
        public async Task AprovarDesembolso(int coControleDesembolso, AprovarDesembolsoRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.AprovarDesembolso(coControleDesembolso, request, cancellationToken);
        public async Task BaixarDRP(int coControleDesembolso, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.BaixarDRP(coControleDesembolso, cancellationToken);
        public async Task RejeitarDesembolso(int coControleDesembolso, RejeitarDesembolsoRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.RejeitarDesembolso(coControleDesembolso, request, cancellationToken);

        public async Task CancelarDesembolso(int coControleDesembolso, CancelarDesembolsoRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.CancelarDesembolso(coControleDesembolso, request, cancellationToken);

        public async Task AtualizarMensagemCefga(int coControleDesembolso, AtualizarMensagemCefgaRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.AtualizarMensagemCefga(coControleDesembolso, request, cancellationToken);

        public async Task<List<ConferenciaCampoResponse>> ExecutarConferenciaCampos(int coControleDesembolso, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ExecutarConferenciaCampos(coControleDesembolso, cancellationToken);
        public async Task VincularResponsavel(int coControleDesembolso, string? matriculaResponsavel, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.VincularResponsavel(coControleDesembolso, matriculaResponsavel, cancellationToken);
        public async Task ValidarDesembolso(int coControleDesembolso, ValidarDesembolsoRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ValidarDesembolso(coControleDesembolso, request, cancellationToken);
        public async Task ValidarTodosPendentes(ValidarDesembolsoRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ValidarTodosPendentes(request, cancellationToken);
        public async Task<List<ValidacaoTemplateResponse>> ObterValidacoesTemplate(CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterValidacoesTemplate(cancellationToken);
        public async Task<PedidoConsultaContratoAfResponse> SolicitarDadosFPD(PedidoConsultaContratoAfRequest pedido, CancellationToken cancellationToken = default)
            => await _fichaPedidoDesembolso.SolicitarDadosFPD(pedido, cancellationToken);
        public async Task CriarFichaPedidoDesembolso(PedidoDesembolsoRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.CriarFichaPedidoDesembolso(request, cancellationToken);
        public async Task ReenviarFichaPedidoDesembolso(int coFpd, PedidoDesembolsoRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ReenviarFichaPedidoDesembolso(coFpd, request, cancellationToken);
        public async Task<List<RegistroDrpResponse>> ObterRegistrosDrp(CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterRegistrosDrp(cancellationToken);
        public async Task BaixarDrpEmLote(BaixarDrpRequest request, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.BaixarDrpEmLote(request, cancellationToken);

        #endregion

        #region Empregado

        public async Task<List<Empregado>> ObterEmpregadosPorCoordenacao(string coordenacao)
            => await _empregados.ObterEmpregadosPorCoordenacao(coordenacao);
        public async Task<List<string>> ObterCodigosGigovPorMatricula(string matricula)
            => await _empregados.ObterCodigosGigovPorMatricula(matricula);

        #endregion
    }
}
