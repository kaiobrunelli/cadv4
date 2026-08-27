using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.AspNetCore.Mvc;
using PlataformaNotificacao.Application.Interface;
using PlataformaOperacional.Application.Service.Interface;

namespace ControleAnaliseDesembolso.Api.Controllers
{
    [ApiController]
    [Route("/api/[action]")]
    public class AplicacaoController : ControllerBase
    {
        private readonly IAplicacaoService _aplicacaoService;
        private readonly INotificacaoService _notificacoes;

        public AplicacaoController(
            IAplicacaoService aplicacaoService,
            INotificacaoService notificacoes)
        {
            _aplicacaoService = aplicacaoService;
            _notificacoes = notificacoes;
        }

        #region Controle Análise Desembolso

        [HttpGet]
        public async Task<ActionResult<List<DesembolsoResponse>>> ObterTodosDesembolsos(CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterTodosDesembolsos(cancellationToken));

        [HttpGet("{coControleDesembolso}")]
        public async Task<ActionResult<DesembolsoDetalheResponse>> ObterDetalheDesembolso(int coControleDesembolso, CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterDetalheDesembolso(coControleDesembolso, cancellationToken));

        [HttpGet("{coControleDesembolso}/comentarios")]
        public async Task<ActionResult<List<ComentarioValidacaoResponse>>> ObterComentarios(int coControleDesembolso, CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterComentarios(coControleDesembolso, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> AdicionarComentario(
            [FromQuery] int coControleDesembolso, [FromQuery] int coValidacao, [FromBody] ValidacaoRegistroRequest registro, CancellationToken cancellationToken)
        {
            await _aplicacaoService.AdicionarComentario(new ValidacaoDesembolsoRequest
            {
                CoControleDesembolso = coControleDesembolso,
                CoValidacao = coValidacao,
                ValidacaoRegistro = registro,
            }, cancellationToken);
            return Ok();
        }

        [HttpPut("{comentarioId}")]
        public async Task<IActionResult> EditarComentario(int comentarioId, [FromBody] EditarComentarioRequest request, CancellationToken cancellationToken)
        {
            request.CoMensagem = comentarioId;
            await _aplicacaoService.EditarComentario(request, cancellationToken);
            return Ok();
        }

        [HttpPut("{coComentario}")]
        public async Task<IActionResult> RemoverComentario(int coComentario, [FromQuery] string matriculaSolicitante, CancellationToken cancellationToken)
        {
            await _aplicacaoService.RemoverComentario(coComentario, matriculaSolicitante, cancellationToken);
            return Ok();
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> Aprovar(int coControleDesembolso, [FromBody] AprovarDesembolsoRequest request, CancellationToken cancellationToken)
        {
            await _aplicacaoService.AprovarDesembolso(coControleDesembolso, request, cancellationToken);
            return Ok();
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> BaixarDRP(int coControleDesembolso, CancellationToken cancellationToken)
        {
            await _aplicacaoService.BaixarDRP(coControleDesembolso, cancellationToken);
            return Ok();
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> Rejeitar(int coControleDesembolso, [FromBody] RejeitarDesembolsoRequest request, CancellationToken cancellationToken)
        {
            await _aplicacaoService.RejeitarDesembolso(coControleDesembolso, request, cancellationToken);
            return Ok();
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> VincularResponsavel(int coControleDesembolso, [FromBody] string? matriculaResponsavel, CancellationToken cancellationToken)
        {
            await _aplicacaoService.VincularResponsavel(coControleDesembolso, matriculaResponsavel, cancellationToken);
            return Ok();
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> RemoverResponsavel(int coControleDesembolso, CancellationToken cancellationToken)
        {
            await _aplicacaoService.VincularResponsavel(coControleDesembolso, null, cancellationToken);
            return Ok();
        }

        [HttpPost("{coControleDesembolso}")]
        public async Task<IActionResult> Validar(int coControleDesembolso, CancellationToken cancellationToken)
        {
            await _aplicacaoService.ValidarDesembolso(coControleDesembolso, cancellationToken);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ValidarTodosPendentes(CancellationToken cancellationToken)
        {
            await _aplicacaoService.ValidarTodosPendentes(cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<ValidacaoTemplateResponse>>> ObterValidacoesTemplate(CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterValidacoesTemplate(cancellationToken));

        [HttpGet]
        public async Task<ActionResult<PedidoConsultaContratoAfResponse>> SolicitarDadosFpd(
            [FromQuery] PedidoConsultaContratoAfRequest pedido, CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.SolicitarDadosFPD(pedido, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> CriarFichaPedidoDesembolso([FromBody] PedidoDesembolsoRequest request, CancellationToken cancellationToken)
        {
            await _aplicacaoService.CriarFichaPedidoDesembolso(request, cancellationToken);
            return Ok();
        }

        [HttpPut("{coFpd}")]
        public async Task<IActionResult> ReenviarFicha(int coFpd, [FromBody] PedidoDesembolsoRequest request, CancellationToken cancellationToken)
        {
            await _aplicacaoService.ReenviarFichaPedidoDesembolso(coFpd, request, cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<RegistroDrpResponse>>> ObterRegistrosDrp(CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterRegistrosDrp(cancellationToken));

        [HttpPut]
        public async Task<IActionResult> BaixarDrp([FromBody] BaixarDrpRequest request, CancellationToken cancellationToken)
        {
            await _aplicacaoService.BaixarDrpEmLote(request, cancellationToken);
            return Ok();
        }

        #endregion

        #region Empregado

        [HttpGet]
        public async Task<ActionResult<List<Empregado>>> ObterEmpregadosPorCoordenacao([FromQuery] string coordenacao)
            => Ok(await _aplicacaoService.ObterEmpregadosPorCoordenacao(coordenacao));

        [HttpGet]
        public async Task<ActionResult<List<string>>> ObterCodigosGigovPorMatricula([FromQuery] string matricula)
            => Ok(await _aplicacaoService.ObterCodigosGigovPorMatricula(matricula));

        #endregion

        #region Notificação

        [HttpGet("/api/notificacao/nao-lidas/total")]
        public async Task<ActionResult<int>> ContarNaoLidas([FromQuery] string usuarioId, CancellationToken cancellationToken)
            => Ok(await _notificacoes.ContarNaoLidasAsync(usuarioId, cancellationToken));

        [HttpGet("/api/notificacao/matricula")]
        public async Task<ActionResult<List<NotificacaoParaSinoResponse>>> ObterNotificacoesPorMatricula([FromQuery] string matricula, CancellationToken cancellationToken)
        {
            var lista = await _notificacoes.ObterNotificacaoPorMatriculaAsync(matricula, cancellationToken: cancellationToken);

            return Ok(lista.Select(n => new NotificacaoParaSinoResponse
            {
                CodigoNotificacao = n.CodigoNotificacao,
                CodigoAplicativo = (int?)n.CodigoAplicativo,
                Titulo = n.Titulo,
                Mensagem = n.Mensagem,
                Tipo = (int)n.Tipo,
                DataCriacao = n.DataCriacao,
                DataValidade = n.DataValidade,
                DataVisualizacao = n.DataVisualizacao,
                Link = n.Link,
            }));
        }

        [HttpPut("/api/notificacao/{codigoNotificacao}/lida")]
        public async Task<IActionResult> MarcarNotificacaoLida(int codigoNotificacao, [FromQuery] string usuarioId, CancellationToken cancellationToken)
        {
            await _notificacoes.MarcarLidaAsync(codigoNotificacao, usuarioId, cancellationToken);
            return Ok();
        }

        [HttpPut("/api/notificacao/marcar-todas-lidas")]
        public async Task<IActionResult> MarcarTodasNotificacoesLidas([FromQuery] string usuarioId, CancellationToken cancellationToken)
        {
            await _notificacoes.MarcarTodasLidasAsync(usuarioId, cancellationToken);
            return Ok();
        }

        #endregion
    }

    public class NotificacaoParaSinoResponse
    {
        public int CodigoNotificacao { get; set; }
        public int? CodigoAplicativo { get; set; }
        public string Titulo { get; set; } = "";
        public string Mensagem { get; set; } = "";
        public int Tipo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataValidade { get; set; }
        public DateTime? DataVisualizacao { get; set; }
        public string? Link { get; set; }
    }
}
