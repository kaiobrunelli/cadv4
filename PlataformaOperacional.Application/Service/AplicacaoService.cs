using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Application.Interface;
using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Interface;
using PlataformaOperacional.Application.Service.Interface;
using RedeCaixaUtilitario.Application.Interface;
using RedeCaixaUtilitario.Domain.Model;

namespace PlataformaOperacional.Application.Service
{
    public class AplicacaoService : IAplicacaoService
    {
        private readonly IControleAnaliseDesembolsoService _controleAnaliseDesembolso;
        private readonly IFichaPedidoDesembolsoService _fichaPedidoDesembolso;
        private readonly IEmpregadoCADService _empregados;
        private readonly IPlataformaOperacionalService _plataformaService;

        private Usuario _usuario => _plataformaService.IdentificarUsuario();

        public AplicacaoService(
            IControleAnaliseDesembolsoService controleAnaliseDesembolso,
            IFichaPedidoDesembolsoService fichaPedidoDesembolso,
            IEmpregadoCADService empregados,
            IPlataformaOperacionalService plataformaService)
        {
            _controleAnaliseDesembolso = controleAnaliseDesembolso;
            _fichaPedidoDesembolso = fichaPedidoDesembolso;
            _empregados = empregados;
            _plataformaService = plataformaService;
        }

        #region Controle Análise Desembolso

        // Endpoints sem PedidoDeAutomacao: o usuário vem direto de _usuario
        // (identificado pela plataforma), não de argumento do controller.
        public async Task<List<DesembolsoResponse>> ObterTodosDesembolsos(CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterTodosDesembolsos(_usuario, cancellationToken);
        public async Task<DesembolsoDetalheResponse> ObterDetalheDesembolso(int coControleDesembolso, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterDetalheDesembolso(coControleDesembolso, _usuario, cancellationToken);
        public async Task<List<ComentarioValidacaoResponse>> ObterComentarios(int coControleDesembolso, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterComentarios(coControleDesembolso, _usuario, cancellationToken);
        public async Task AdicionarComentario(ValidacaoDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.AdicionarComentario(request, pedidoAutomacao, cancellationToken);
        public async Task EditarComentario(EditarComentarioRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.EditarComentario(request, pedidoAutomacao, cancellationToken);
        public async Task RemoverComentario(int coRegistroValidacao, string matriculaSolicitante, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.RemoverComentario(coRegistroValidacao, matriculaSolicitante, pedidoAutomacao, cancellationToken);
        public async Task AprovarDesembolso(int coControleDesembolso, AprovarDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.AprovarDesembolso(coControleDesembolso, request, pedidoAutomacao, cancellationToken);
        public async Task BaixarDRP(int coControleDesembolso, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.BaixarDRP(coControleDesembolso, pedidoAutomacao, cancellationToken);
        public async Task RejeitarDesembolso(int coControleDesembolso, RejeitarDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.RejeitarDesembolso(coControleDesembolso, request, pedidoAutomacao, cancellationToken);

        public async Task CancelarDesembolso(int coControleDesembolso, CancelarDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.CancelarDesembolso(coControleDesembolso, request, pedidoAutomacao, cancellationToken);

        public async Task AtualizarMensagemCefga(int coControleDesembolso, AtualizarMensagemCefgaRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.AtualizarMensagemCefga(coControleDesembolso, request, pedidoAutomacao, cancellationToken);

        public async Task<List<ConferenciaCampoResponse>> ExecutarConferenciaCampos(int coControleDesembolso, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ExecutarConferenciaCampos(coControleDesembolso, pedidoAutomacao, cancellationToken);
        public async Task VincularResponsavel(int coControleDesembolso, string? matriculaResponsavel, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.VincularResponsavel(coControleDesembolso, matriculaResponsavel, pedidoAutomacao, cancellationToken);
        public async Task ValidarDesembolso(int coControleDesembolso, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ValidarDesembolso(coControleDesembolso, pedidoAutomacao, cancellationToken);
        public async Task ValidarTodosPendentes(PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ValidarTodosPendentes(pedidoAutomacao, cancellationToken);
        public async Task<List<ValidacaoTemplateResponse>> ObterValidacoesTemplate(CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterValidacoesTemplate(_usuario, cancellationToken);
        public async Task<PedidoConsultaContratoAfResponse> SolicitarDadosFPD(PedidoConsultaContratoAfRequest pedido, CancellationToken cancellationToken = default)
            => await _fichaPedidoDesembolso.SolicitarDadosFPD(pedido, cancellationToken);
        public async Task CriarFichaPedidoDesembolso(PedidoDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.CriarFichaPedidoDesembolso(request, pedidoAutomacao, cancellationToken);
        public async Task ReenviarFichaPedidoDesembolso(int coFpd, PedidoDesembolsoRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ReenviarFichaPedidoDesembolso(coFpd, request, pedidoAutomacao, cancellationToken);
        public async Task<List<RegistroDrpResponse>> ObterRegistrosDrp(CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.ObterRegistrosDrp(_usuario, cancellationToken);
        public async Task BaixarDrpEmLote(BaixarDrpRequest request, PedidoDeAutomacao pedidoAutomacao, CancellationToken cancellationToken = default)
            => await _controleAnaliseDesembolso.BaixarDrpEmLote(request, pedidoAutomacao, cancellationToken);

        #endregion

        #region Empregado

        public async Task<List<Empregado>> ObterEmpregadosPorCoordenacao(string coordenacao)
            => await _empregados.ObterEmpregadosPorCoordenacao(coordenacao);
        public async Task<List<string>> ObterCodigosGigovPorMatricula(string matricula)
            => await _empregados.ObterCodigosGigovPorMatricula(matricula);

        #endregion
    }
}
