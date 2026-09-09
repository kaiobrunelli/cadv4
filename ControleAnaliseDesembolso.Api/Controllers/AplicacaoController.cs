using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.AspNetCore.Mvc;
using PlataformaNotificacao.Application.Interface;
using PlataformaOperacional.Application.Service.Interface;
using RedeCaixaUtilitario.Application.Interface;
using RedeCaixaUtilitario.Domain.Model;

namespace ControleAnaliseDesembolso.Api.Controllers
{
    [ApiController]
    [Route("/api/[action]")]
    public class AplicacaoController : ControllerBase
    {
        private readonly IAplicacaoService _aplicacaoService;
        private readonly INotificacaoService _notificacoes;
        private readonly IPlataformaOperacionalService _plataformaService;

        public AplicacaoController(
            IAplicacaoService aplicacaoService,
            INotificacaoService notificacoes,
            IPlataformaOperacionalService plataformaService)
        {
            _aplicacaoService = aplicacaoService;
            _notificacoes = notificacoes;
            _plataformaService = plataformaService;
        }

        private string IdentificaEndpoint() => HttpContext.GetEndpoint()?.DisplayName ?? string.Empty;

        // Monta, num só lugar, a trilha de auditoria (nível de chamada) e o
        // PedidoDeAutomacao de cada ação — é a plataforma (_plataformaService)
        // quem identifica o usuário a partir da senha informada, não o request.
        private (TrilhaDeAuditoria Trilha, PedidoDeAutomacao Pedido) PrepararAuditoriaEPedido(string evento, string descEvento, string senha) =>
        (
            _plataformaService.GerarTrilhaDeAuditoria(evento, descEvento, "Solicitado", IdentificaEndpoint()),
            _plataformaService.GerarPedidoDeAutomacao(this.GetType().Name.Replace("Service", ""), IdentificaEndpoint(), senha, false)
        );

        #region Controle Análise Desembolso

        // Sem PedidoDeAutomacao (não são ação/automação) — IAplicacaoService já
        // resolve o usuário internamente via _usuario, então o controller não muda.

        [HttpGet]
        public async Task<ActionResult<List<DesembolsoResponse>>> ObterTodosDesembolsos(CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterTodosDesembolsos(cancellationToken));

        [HttpGet("{coControleDesembolso}")]
        public async Task<ActionResult<DesembolsoDetalheResponse>> ObterDetalheDesembolso(int coControleDesembolso, CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterDetalheDesembolso(coControleDesembolso, cancellationToken));

        [HttpGet("{coControleDesembolso}/comentarios")]
        public async Task<ActionResult<List<ComentarioValidacaoResponse>>> ObterComentarios(int coControleDesembolso, CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterComentarios(coControleDesembolso, cancellationToken));

        [HttpGet]
        public async Task<ActionResult<List<ValidacaoTemplateResponse>>> ObterValidacoesTemplate(CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterValidacoesTemplate(cancellationToken));

        [HttpGet]
        public async Task<ActionResult<List<RegistroDrpResponse>>> ObterRegistrosDrp(CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.ObterRegistrosDrp(cancellationToken));

        [HttpGet]
        public async Task<ActionResult<PedidoConsultaContratoAfResponse>> SolicitarDadosFpd(
            [FromQuery] PedidoConsultaContratoAfRequest pedido, CancellationToken cancellationToken)
            => Ok(await _aplicacaoService.SolicitarDadosFPD(pedido, cancellationToken));

        // Com PedidoDeAutomacao — usuário vem da senha informada, via
        // _plataformaService (a plataforma injeta o usuário a partir daqui).

        [HttpPost]
        public async Task<IActionResult> AdicionarComentario(
            [FromQuery] int coControleDesembolso, [FromQuery] int coValidacao, [FromBody] ValidacaoRegistroRequest registro,
            [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido(
                "Adicionar comentário", $"Comentou a validação {coValidacao} do desembolso {coControleDesembolso}", senha);
            try
            {
                await _aplicacaoService.AdicionarComentario(new ValidacaoDesembolsoRequest
                {
                    CoControleDesembolso = coControleDesembolso,
                    CoValidacao = coValidacao,
                    ValidacaoRegistro = registro,
                }, pedido, cancellationToken);

                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{comentarioId}")]
        public async Task<IActionResult> EditarComentario(
            int comentarioId, [FromBody] EditarComentarioRequest request, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            request.CoMensagem = comentarioId;
            var (trilha, pedido) = PrepararAuditoriaEPedido("Editar comentário", $"Editou o comentário {comentarioId}", senha);
            try
            {
                await _aplicacaoService.EditarComentario(request, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{coComentario}")]
        public async Task<IActionResult> RemoverComentario(
            int coComentario, [FromQuery] string matriculaSolicitante, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido("Remover comentário", $"Removeu o comentário {coComentario}", senha);
            try
            {
                await _aplicacaoService.RemoverComentario(coComentario, matriculaSolicitante, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> Aprovar(
            int coControleDesembolso, [FromBody] AprovarDesembolsoRequest request, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido("Aprovar desembolso", $"Aprovou o desembolso {coControleDesembolso}", senha);
            try
            {
                await _aplicacaoService.AprovarDesembolso(coControleDesembolso, request, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> BaixarDRP(int coControleDesembolso, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido("Baixar DRP", $"Baixou a DRP do desembolso {coControleDesembolso}", senha);
            try
            {
                await _aplicacaoService.BaixarDRP(coControleDesembolso, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> Rejeitar(
            int coControleDesembolso, [FromBody] RejeitarDesembolsoRequest request, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido("Rejeitar desembolso", $"Rejeitou o desembolso {coControleDesembolso}", senha);
            try
            {
                await _aplicacaoService.RejeitarDesembolso(coControleDesembolso, request, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> Cancelar(
            int coControleDesembolso, [FromBody] CancelarDesembolsoRequest request, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido("Cancelar desembolso", $"Cancelou o desembolso {coControleDesembolso}", senha);
            try
            {
                await _aplicacaoService.CancelarDesembolso(coControleDesembolso, request, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> AtualizarMensagemCefga(
            int coControleDesembolso, [FromBody] AtualizarMensagemCefgaRequest request, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido("Editar OBS CEFGA", $"Atualizou o OBS CEFGA do desembolso {coControleDesembolso}", senha);
            try
            {
                await _aplicacaoService.AtualizarMensagemCefga(coControleDesembolso, request, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPost("{coControleDesembolso}")]
        public async Task<ActionResult<List<ConferenciaCampoResponse>>> ExecutarConferenciaCampos(
            int coControleDesembolso, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido(
                "Executar conferência de campos", $"Rodou a conferência de campos do desembolso {coControleDesembolso}", senha);
            try
            {
                var resultado = await _aplicacaoService.ExecutarConferenciaCampos(coControleDesembolso, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> VincularResponsavel(
            int coControleDesembolso, [FromBody] string? matriculaResponsavel, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido(
                "Vincular responsável", $"Vinculou {matriculaResponsavel} como responsável pela análise do desembolso {coControleDesembolso}", senha);
            try
            {
                await _aplicacaoService.VincularResponsavel(coControleDesembolso, matriculaResponsavel, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{coControleDesembolso}")]
        public async Task<IActionResult> RemoverResponsavel(int coControleDesembolso, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido(
                "Remover responsável", $"Removeu o responsável pela análise do desembolso {coControleDesembolso}", senha);
            try
            {
                await _aplicacaoService.VincularResponsavel(coControleDesembolso, null, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPost("{coControleDesembolso}")]
        public async Task<IActionResult> Validar(int coControleDesembolso, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido("Validar", $"Validou o desembolso {coControleDesembolso}", senha);
            try
            {
                await _aplicacaoService.ValidarDesembolso(coControleDesembolso, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidarTodosPendentes([FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido("Validar todos pendentes", "Validou em lote os desembolsos pendentes", senha);
            try
            {
                await _aplicacaoService.ValidarTodosPendentes(pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CriarFichaPedidoDesembolso(
            [FromBody] PedidoDesembolsoRequest request, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido(
                "Criar FPD", $"Solicitou novo desembolso pro contrato {request.CoContratoAf}-{request.CoContratoAfDv}", senha);
            try
            {
                await _aplicacaoService.CriarFichaPedidoDesembolso(request, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut("{coFpd}")]
        public async Task<IActionResult> ReenviarFicha(
            int coFpd, [FromBody] PedidoDesembolsoRequest request, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido("Reenviar FPD", $"Reenviou a FPD {coFpd} pra nova análise", senha);
            try
            {
                await _aplicacaoService.ReenviarFichaPedidoDesembolso(coFpd, request, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
        }

        [HttpPut]
        public async Task<IActionResult> BaixarDrp([FromBody] BaixarDrpRequest request, [FromQuery] string senha, CancellationToken cancellationToken)
        {
            var (trilha, pedido) = PrepararAuditoriaEPedido(
                "Baixar DRP em lote", $"Baixou a DRP de {request.Ids.Count} desembolso(s): {string.Join(", ", request.Ids)}", senha);
            try
            {
                await _aplicacaoService.BaixarDrpEmLote(request, pedido, cancellationToken);
                trilha.Resposta = "ACATADO";
                return Ok();
            }
            catch (Exception ex)
            {
                trilha.Resposta = $"ERRO: {ex.Message}";
                throw;
            }
            finally
            {
                await _plataformaService.SaveAsync();
            }
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
