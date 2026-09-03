using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Application.Interface;
using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Enums;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using ControleAnaliseDesembolso.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PlataformaNotificacao.Application.Interface;
using PlataformaNotificacao.Domain.Enum;
using RedeCaixaUtilitario.Application.Interface;
using RedeCaixaUtilitario.Domain.Model;
using Utilitarios.Service;

namespace ControleAnaliseDesembolso.Application
{
    public class ControleAnaliseDesembolsoService : IControleAnaliseDesembolsoService
    {
        private readonly ControleAnaliseDesembolsoContext _context;
        private readonly IValidadorDesembolsoService _validador;
        private readonly IEmpregadoCADService _empregados;
        private readonly INotificacaoService _notificacoes;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UtilitarioMapperServicecopy _mapperCopy;
        private readonly ISiapfService _siapf;
        private readonly IRepositorioDesembolso _repositorioDesembolso;
        private readonly IRepositorioControle _repositorioControle;
        private readonly IRepositorioMensagem _repositorioMensagem;
        private readonly IRepositorioValidacao _repositorioValidacao;
        private readonly IRepositorioValidacaoControle _repositorioValidacaoControle;

        private const string CodigoCoordenacaoCefga = "CEFGA06";

        public ControleAnaliseDesembolsoService(
            ControleAnaliseDesembolsoContext context,
            IValidadorDesembolsoService validador,
            IEmpregadoCADService empregados,
            INotificacaoService notificacoes,
            IHttpContextAccessor httpContextAccessor,
            UtilitarioMapperServicecopy mapperCopy,
            ISiapfService siapf,
            IRepositorioDesembolso repositorioDesembolso,
            IRepositorioControle repositorioControle,
            IRepositorioMensagem repositorioMensagem,
            IRepositorioValidacao repositorioValidacao,
            IRepositorioValidacaoControle repositorioValidacaoControle)
        {
            _context = context;
            _validador = validador;
            _empregados = empregados;
            _notificacoes = notificacoes;
            _httpContextAccessor = httpContextAccessor;
            _mapperCopy = mapperCopy;
            _siapf = siapf;
            _repositorioDesembolso = repositorioDesembolso;
            _repositorioControle = repositorioControle;
            _repositorioMensagem = repositorioMensagem;
            _repositorioValidacao = repositorioValidacao;
            _repositorioValidacaoControle = repositorioValidacaoControle;
        }

   
        private void RegistrarAuditoria(string? usuario, string evento, string descEvento, string? resposta = "ACATADO")
        {
            var enderecoLogico = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            _context.TrilhaAuditoria.Add(new TrilhaAuditoria
            {
                Usuario = string.IsNullOrWhiteSpace(usuario) ? "SISTEMA" : usuario,
                EnderecoLogicoSolicitante = string.IsNullOrWhiteSpace(enderecoLogico) ? "desconhecido" : enderecoLogico,
                DataSolicitacao = DateTime.Now,
                Evento = evento,
                DescEvento = descEvento,
                Resposta = resposta,
            });
        }

  
        private static string LinkDesembolso(int coControleDesembolso) => $"/cad?contratoId={coControleDesembolso}";
     
        public async Task<string?> ObterREsponsavelDesembolso(int coFpd, CancellationToken cancellationToken = default)
        {
            var fpd = await _repositorioDesembolso.ObterDesembolso(coFpd, cancellationToken);

            if (fpd is null)
                throw new Exception($"Desembolso {coFpd} não encontrado.");

            return fpd.ControleDesembolso.ResponsavelAnalise;
        }

        public async Task<List<ComentarioValidacaoResponse>> ObterComentarios(int coControleDesembolso, CancellationToken cancellationToken = default)
        {
            var existe = await _repositorioControle.ExisteControleDesembolso(coControleDesembolso, cancellationToken);
            if (!existe)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            return await _repositorioMensagem.ObterComentario(coControleDesembolso, cancellationToken);
        }

        public async Task<DesembolsoDetalheResponse> ObterDetalheDesembolso(int coControleDesembolso, CancellationToken cancellationToken = default)
        {
            var desembolso = await _repositorioControle.ObterControleDesembolsoCompleto(coControleDesembolso, cancellationToken);

            if (desembolso is null)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            var comentarios = await _repositorioMensagem.ObterComentario(coControleDesembolso, cancellationToken);

            var comentariosPorValidacao = comentarios
                .GroupBy(c => c.CoValidacao)
                .ToDictionary(g => g.Key, g => g.ToList());

            var coValidacoesDoChecklist = desembolso.ValidacaoControleDesembolso
                .Select(v => v.CoValidacao)
                .ToHashSet();

            var fpd = desembolso.Desembolso;

            return new DesembolsoDetalheResponse
            {
                CoControleDesembolso = desembolso.CoControleDesembolso,
                CoDesembolso = desembolso.CoDesembolso,
                Status = desembolso.StatusDesembolso,
                DtSolicitado = fpd.DtSolicitado,
                DtPrazo = desembolso.DtPrazo,
                DtConclusao = desembolso.DtConclusao,
                ResponsavelAnalise = desembolso.ResponsavelAnalise,
                ResponsavelBaixa = desembolso.ResponsavelBaixa,
                NuDesembolso = fpd.NuDesembolso,
                CndValido = fpd.CndValido,
                CrpValido = fpd.CrpValido,
                CrpNsa = fpd.CrpNsa,
                Mensagem = fpd.Mensagem,
                MotivoRejeicao = fpd.MotivoRejeicao,
                CoContratoAf = fpd.CoContratoAf,
                CoContratoAfDv = fpd.CoContratoAfDv,
                ContratoAo = fpd.ContratoAo,
                ContratoAoDv = fpd.ContratoAoDv,
                CoGigov = fpd.CoGigov,
                MutuarioFinal = fpd.MutuarioFinal,
                CnpjMutuarioFinal = fpd.CnpjMutuarioFinal,
                AgenteFinanceiro = fpd.AgenteFinanceiro,
                AgentePromotor = fpd.AgentePromotor,
                Programa = fpd.Programa.ToString(),
                TipoDesembolso = fpd.TipoDesembolso.ToString(),
                PrimeiroDesembolso = fpd.PrimeiroDesembolso,
                UltimoDesembolso = fpd.UltimoDesembolso,
                Recorrente = fpd.Recorrente,
                PercentualObra = fpd.PercentualObra,
                ValorEmprestimo = fpd.ValorEmprestimo,
                SolicitadoVi = fpd.SolicitadoVi,
                ParticipacaoFgts = fpd.ParticipacaoFgts,
                Contrapartida = fpd.Contrapartida,
                MatriculaSolicitante = fpd.MatriculaSolicitante,
                MatriculaGestor = fpd.MatriculaGestor,
                CnpjAf = fpd.CnpjAf,
                AgenteTecnicoOperador = fpd.AgenteTecnicoOperador,
                CnpjAgenteTecnicoOperador = fpd.CnpjAgenteTecnicoOperador,
                CnpjAgentePromotor = fpd.CnpjAgentePromotor,
                DtEngenharia = fpd.DtEngenharia,
                SituacaoObra = fpd.SituacaoObra?.ToString(),
                DtSocioAmbiental = fpd.DtSocioAmbiental,
                Concluido = fpd.Concluido,
                GlossadoVi = fpd.GlossadoVi,
                AceitoVi = fpd.AceitoVi,
                Desembolsado = fpd.Desembolsado,
                SaldoDesembolsar = fpd.SaldoDesembolsar,
                Excepcionalizado = fpd.Excepcionalizado,
                ContrapartidaAtual = fpd.ContrapartidaAtual,
                Integralizado = fpd.Integralizado,
                SaldoIntegralizar = fpd.SaldoIntegralizar,
                ContrapartidaAlterada = fpd.ContrapartidaAlterada,
                Sanepar = fpd.Sanepar,
                RetornoParcial = fpd.RetornoParcial,
                PlacaLocal = fpd.PlacaLocal,
                LicensaInstalacao = fpd.LicensaInstalacao,
                LicensaOperacao = fpd.LicensaOperacao,
                Funcionalidade = fpd.Funcionalidade,
                TemCarroceria = fpd.TemCarroceria,
                VeiculoPossuiAdesivos = fpd.VeiculoPossuiAdesivos,
                DataInicioObra = fpd.DataInicioObra,
                DestinacaoColetaResiduosSolidos = fpd.DestinacaoColetaResiduosSolidos,
                MotivoCancelamento = desembolso.MotivoCancelamento,
                NumeroDrp = desembolso.NumeroDrp,
                DvDrp = desembolso.DvDrp,
                SenhaDrp = desembolso.SenhaDrp,
                DtDrp = desembolso.DtDrp,
                CrfAf = desembolso.CrfAf,
                CrfTomador = desembolso.CrfTomador,
                CrfAp = desembolso.CrfAp,
                CrfAt = desembolso.CrfAt,
                MensagemCefga = fpd.MensagemCefga,
                DtUltimaConferencia = desembolso.DtUltimaConferencia,
                ConferenciaCampos = desembolso.Conferencias
                    .OrderBy(c => c.CoCampo)
                    .Select(c => new ConferenciaCampoResponse
                    {
                        CoCampo = c.CoCampo,
                        DeCampo = c.DeCampo,
                        Situacao = c.Situacao.ToString(),
                        Mensagem = c.Mensagem,
                    })
                    .ToList(),
                Checklist = desembolso.ValidacaoControleDesembolso
                    .OrderBy(v => v.CoValidacao)
                    .Select(v => new ChecklistItemResponse
                    {
                        CoValidacao = v.CoValidacao,
                        DeValidacao = v.Validacao?.DeValidacao ?? v.DeValidacao,
                        Situacao = v.Situacao,
                        Comentarios = comentariosPorValidacao.TryGetValue(v.CoValidacao, out var lista) ? lista : new(),
                    })
                    .ToList(),
                ComentariosGerais = comentarios
                    .Where(c => !coValidacoesDoChecklist.Contains(c.CoValidacao))
                    .ToList(),
            };
        }



        public async Task CriarFichaPedidoDesembolso(PedidoDesembolsoRequest request, CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            int coControleDesembolso;

            try
            {
                var fpd = _mapperCopy.Map<Desembolso>(request);
                fpd.DtSolicitado = DateTime.UtcNow;
                fpd.ControleDesembolso = new ControleDesembolso
                {
                    DtPrazo = AdicionarDiasUteis(DateTime.Today),
                    StatusDesembolso = TipoStatusDesembolso.PENDENTE,
                    ResponsavelAnalise = null,
                    ResponsavelBaixa = null,
                    Gestor = null,
                    NumeroDrp = request.NumeroDrp,
                    DvDrp = request.DvDrp,
                    SenhaDrp = request.SenhaDrp,
                    DtDrp = request.DtDrp,
                    CrfAf = request.CrfAf,
                    CrfTomador = request.CrfTomador,
                    CrfAp = request.CrfAp,
                    CrfAt = request.CrfAt,
                };

                await _repositorioDesembolso.Adicionar(fpd, cancellationToken);

                coControleDesembolso = fpd.ControleDesembolso.CoControleDesembolso;

                var validacoesModelo = await _repositorioValidacao.TrazerValidacao(cancellationToken) ?? [];

                var registrosPorValidacao = request.ValidacoesDesembolsoRequest
                    .Where(x => x.ValidacaoRegistro is not null &&
                                !string.IsNullOrWhiteSpace(x.ValidacaoRegistro.DeMensagem))
                    .GroupBy(x => x.CoValidacao)
                    .ToDictionary(x => x.Key, x => x.ToList());

                foreach (var modelo in validacoesModelo)
                {
                    registrosPorValidacao.TryGetValue(modelo.CoValidacao, out var registrosDoItem);

                    var temParecer = registrosDoItem?.Any(
                        r => r.ValidacaoRegistro.TipoMensagem == TipoMensagem.PARECER) ?? false;

                    var validacaoDesembolso = new ValidacaoControleDesembolso
                    {
                        CoValidacao = modelo.CoValidacao,
                        CoControleDesembolso = coControleDesembolso,
                        DeValidacao = modelo.DeValidacao,
                        CampoVinculado = null,
                        Situacao = temParecer ? TipoSituacaoValidacao.APROVADO : TipoSituacaoValidacao.ANALISAR,
                    };

                    await _repositorioValidacaoControle.AdicionarSemSalvar(validacaoDesembolso, cancellationToken);

                    if (registrosDoItem is null) continue;

                    foreach (var validacaoDesembolsoRequest in registrosDoItem)
                    {
                        var registro = new Mensagem()
                        {
                            CoValidacao = validacaoDesembolsoRequest.CoValidacao,
                            CoControleDesembolso = coControleDesembolso,
                            CoUsuario = validacaoDesembolsoRequest.ValidacaoRegistro.MatriculaAutor,
                            DeUsuario = validacaoDesembolsoRequest.ValidacaoRegistro.NomeAutor,
                            UnidadeUsuario = validacaoDesembolsoRequest.ValidacaoRegistro.UnidadeAutor,
                            DtCriacao = DateTime.Now,
                            DeMensagem = validacaoDesembolsoRequest.ValidacaoRegistro.DeMensagem,
                            TipoMensagem = validacaoDesembolsoRequest.ValidacaoRegistro.TipoMensagem,
                        };

                        await _repositorioMensagem.AdicionarSemSalvar(registro, cancellationToken);
                    }
                }

                RegistrarAuditoria(request.MatriculaSolicitante, "Criar FPD",
                    $"Solicitou novo desembolso pro contrato {request.CoContratoAf}-{request.CoContratoAfDv}");

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                var erroCompleto = ex.InnerException?.InnerException?.Message
                    ?? ex.InnerException?.Message
                    ?? ex.Message;
                throw new Exception($"Erro: {erroCompleto}");
            }

            await _notificacoes.EnviarCoordenacaoAsync(
                CodigoCoordenacaoCefga,
                "Novo desembolso inserido",
                $"Contrato {request.CoContratoAf}-{request.CoContratoAfDv} entrou na fila de análise.",
                link: LinkDesembolso(coControleDesembolso),
                cancellationToken: cancellationToken);
        }

        public async Task ReenviarFichaPedidoDesembolso(int coFpd, PedidoDesembolsoRequest request, CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            Desembolso fpd;

            try
            {
                fpd = await _repositorioDesembolso.ObterDesembolso(coFpd, cancellationToken);

                if (fpd is null)
                    throw new Exception($"FPD {coFpd} não encontrada.");

                AtualizarDadosFicha(fpd, request);

                fpd.ControleDesembolso.StatusDesembolso = TipoStatusDesembolso.PENDENTE;
                fpd.ControleDesembolso.DtConclusao = null;

                fpd.ControleDesembolso.DtPrazo = AdicionarDiasUteis(DateTime.Today);

                foreach (var validacao in fpd.ControleDesembolso.ValidacaoControleDesembolso)
                {
                    validacao.Situacao = TipoSituacaoValidacao.ANALISAR;
                }

                var coValidacaoJaExistentes = fpd.ControleDesembolso.ValidacaoControleDesembolso
                    .Select(v => v.CoValidacao)
                    .ToHashSet();

                var novosModelos = await _context.Validacao
                    .Where(x => !x.Desativado && !coValidacaoJaExistentes.Contains(x.CoValidacao))
                    .ToListAsync(cancellationToken);

                foreach (var modelo in novosModelos)
                {
                    await _repositorioValidacaoControle.AdicionarSemSalvar(new ValidacaoControleDesembolso
                    {
                        CoValidacao = modelo.CoValidacao,
                        CoControleDesembolso = fpd.ControleDesembolso.CoControleDesembolso,
                        DeValidacao = modelo.DeValidacao,
                        Situacao = TipoSituacaoValidacao.ANALISAR,
                    }, cancellationToken);
                }

                var registrosPorValidacao = request.ValidacoesDesembolsoRequest
                    .Where(x => x.ValidacaoRegistro is not null &&
                                !string.IsNullOrWhiteSpace(x.ValidacaoRegistro.DeMensagem))
                    .GroupBy(x => x.CoValidacao)
                    .ToDictionary(x => x.Key, x => x.ToList());

                foreach (var validacao in fpd.ControleDesembolso.ValidacaoControleDesembolso)
                {
                    if (!registrosPorValidacao.TryGetValue(validacao.CoValidacao, out var registrosDoItem))
                    {
                        continue;
                    }

                    foreach (var validacaoRequest in registrosDoItem)
                    {
                        var registro = new Mensagem
                        {
                            CoValidacao = validacao.CoValidacao,
                            CoControleDesembolso = validacao.CoControleDesembolso,
                            DeMensagem = validacaoRequest.ValidacaoRegistro.DeMensagem,
                            TipoMensagem = validacaoRequest.ValidacaoRegistro.TipoMensagem,
                            CoUsuario = validacaoRequest.ValidacaoRegistro.MatriculaAutor,
                            DeUsuario = validacaoRequest.ValidacaoRegistro.NomeAutor,
                            UnidadeUsuario = validacaoRequest.ValidacaoRegistro.UnidadeAutor,
                            DtCriacao = DateTime.Now,
                        };
                        await _repositorioMensagem.AdicionarSemSalvar(registro, cancellationToken);
                    }
                }

                RegistrarAuditoria(request.MatriculaSolicitante, "Reenviar FPD",
                    $"Reenviou a FPD {coFpd} pra nova análise");

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception($"Erro ao reenviar FPD {coFpd}: {ex.Message}");
            }

            await _notificacoes.EnviarCoordenacaoAsync(
                CodigoCoordenacaoCefga,
                "Ficha reenviada para nova análise",
                $"Contrato {fpd.CoContratoAf}-{fpd.CoContratoAfDv} foi corrigido e voltou para análise.",
                link: LinkDesembolso(fpd.ControleDesembolso.CoControleDesembolso),
                cancellationToken: cancellationToken);
        }

        private static void AtualizarDadosFicha(Desembolso fpd, PedidoDesembolsoRequest request)
        {
            fpd.MatriculaSolicitante = request.MatriculaSolicitante;
            fpd.CoGigov = request.CoGigov;
            fpd.MatriculaGestor = request.MatriculaGestor;
            fpd.NuDesembolso = request.NuDesembolso;
            fpd.CoContratoAf = request.CoContratoAf;
            fpd.CoContratoAfDv = request.CoContratoAfDv;
            fpd.PrimeiroDesembolso = request.PrimeiroDesembolso;
            fpd.Recorrente = request.Recorrente;
            fpd.AgenteFinanceiro = request.AgenteFinanceiro;
            fpd.CnpjAf = request.CnpjAf;
            fpd.MutuarioFinal = request.MutuarioFinal;
            fpd.CnpjMutuarioFinal = request.CnpjMutuarioFinal;
            fpd.AgenteTecnicoOperador = request.AgenteTecnicoOperador;
            fpd.CnpjAgenteTecnicoOperador = request.CnpjAgenteTecnicoOperador;
            fpd.AgentePromotor = request.AgentePromotor;
            fpd.CnpjAgentePromotor = request.CnpjAgentePromotor;
            fpd.Programa = (Programa)request.Programa;
            fpd.UltimoDesembolso = request.UltimoDesembolso;
            fpd.Funcionalidade = request.Funcionalidade;
            fpd.Concluido = request.Concluido;
            fpd.DtEngenharia = request.DtEngenharia;
            fpd.SituacaoObra = (TipoSituacaoObra)request.SituacaoObra;
            fpd.DtSocioAmbiental = request.DtSocioAmbiental;
            fpd.PercentualObra = request.PercentualObra;
            fpd.TipoDesembolso = (TipoDesembolso)request.TipoDesembolso;
            fpd.RetornoParcial = request.RetornoParcial;
            fpd.PlacaLocal = request.PlacaLocal;
            fpd.LicensaInstalacao = request.LicensaInstalacao;
            fpd.LicensaOperacao = request.LicensaOperacao;
            fpd.CndValido = request.CndValido;
            fpd.CrpValido = request.CrpValido;
            fpd.CrpNsa = request.CrpNsa;
            fpd.SolicitadoVi = request.SolicitadoVi;
            fpd.GlossadoVi = request.GlossadoVi;
            fpd.AceitoVi = request.AceitoVi;
            fpd.ParticipacaoFgts = request.ParticipacaoFgts;
            fpd.Contrapartida = request.Contrapartida;
            fpd.ValorEmprestimo = request.ValorEmprestimo;
            fpd.Desembolsado = request.Desembolsado;
            fpd.SaldoDesembolsar = request.SaldoDesembolsar;
            fpd.Excepcionalizado = request.Excepcionalizado;
            fpd.ContrapartidaAtual = request.ContrapartidaAtual;
            fpd.Integralizado = request.Integralizado;
            fpd.SaldoIntegralizar = request.SaldoIntegralizar;
            fpd.ContrapartidaAlterada = request.ContrapartidaAlterada;
            fpd.Sanepar = request.Sanepar;
            fpd.Mensagem = request.Mensagem;
            fpd.TemCarroceria = request.TemCarroceria;
            fpd.VeiculoPossuiAdesivos = request.VeiculoPossuiAdesivos;
            fpd.DataInicioObra = request.DataInicioObra;
            fpd.DestinacaoColetaResiduosSolidos = request.DestinacaoColetaResiduosSolidos;
            fpd.ControleDesembolso.NumeroDrp = request.NumeroDrp;
            fpd.ControleDesembolso.DvDrp = request.DvDrp;
            fpd.ControleDesembolso.SenhaDrp = request.SenhaDrp;
            fpd.ControleDesembolso.DtDrp = request.DtDrp;
            fpd.ControleDesembolso.CrfAf = request.CrfAf;
            fpd.ControleDesembolso.CrfTomador = request.CrfTomador;
            fpd.ControleDesembolso.CrfAp = request.CrfAp;
            fpd.ControleDesembolso.CrfAt = request.CrfAt;
        }

        public async Task AdicionarComentario(ValidacaoDesembolsoRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.ValidacaoRegistro is null ||
                string.IsNullOrWhiteSpace(request.ValidacaoRegistro.DeMensagem))
            {
                throw new Exception("O comentário não pode ser vazio.");
            }

            var validacao = await _repositorioValidacaoControle.BuscarValidacao(request.CoValidacao, request.CoControleDesembolso, cancellationToken);

            if (validacao is null)
                throw new Exception($"Validação {request.CoValidacao} do desembolso {request.CoControleDesembolso} não encontrada.");


            if (request.ValidacaoRegistro.TipoMensagem == TipoMensagem.PARECER)
            {
                validacao.Situacao = TipoSituacaoValidacao.APROVADO;
            }

            var registro = new Mensagem
            {
                CoValidacao = request.CoValidacao,
                CoControleDesembolso = request.CoControleDesembolso,

                DeMensagem = request.ValidacaoRegistro.DeMensagem.Trim(),
                TipoMensagem = request.ValidacaoRegistro.TipoMensagem,
                CoUsuario = request.ValidacaoRegistro.MatriculaAutor,
                DeUsuario = request.ValidacaoRegistro.NomeAutor,
                UnidadeUsuario = request.ValidacaoRegistro.UnidadeAutor,
                DtCriacao = DateTime.Now,
            };

            await _repositorioMensagem.AdicionarSemSalvar(registro, cancellationToken);

            RegistrarAuditoria(request.ValidacaoRegistro.MatriculaAutor, "Adicionar comentário",
                $"Comentou a validação {request.CoValidacao} do desembolso {request.CoControleDesembolso}");

            await _context.SaveChangesAsync(cancellationToken);

            await NotificarComentarioInserido(request.CoControleDesembolso, request.ValidacaoRegistro.UnidadeAutor, cancellationToken);
        }

        private async Task NotificarComentarioInserido(int coControleDesembolso, int unidadeAutor, CancellationToken cancellationToken = default)
        {
            var controle = await _context.ControleDesembolso
                .Where(x => x.CoControleDesembolso == coControleDesembolso)
                .Select(x => new
                {
                    Contrato = x.Desembolso.CoContratoAf + "-" + x.Desembolso.CoContratoAfDv,
                    x.Desembolso.CoGigov,
                    x.ResponsavelAnalise,
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (controle is null)
                return;

            if (unidadeAutor == 7175)
            {
                if (string.IsNullOrWhiteSpace(controle.CoGigov))
                    return;

                await _notificacoes.EnviarParaGigovAsync(
                    controle.CoGigov,
                    $"Novo comentário no contrato {controle.Contrato}",
                    "A CEFGA registrou um comentário que precisa da sua atenção.",
                    CodigoAplicativo.Cad,
                    link: LinkDesembolso(coControleDesembolso),
                    cancellationToken: cancellationToken);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(controle.ResponsavelAnalise))
                {
                    await _notificacoes.EnviarPorMatriculasAsync(
                        [controle.ResponsavelAnalise],
                        $"Novo comentário no contrato {controle.Contrato}",
                        "O solicitante (GIGOV) registrou um comentário no desembolso.",
                        link: LinkDesembolso(coControleDesembolso),
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await _notificacoes.EnviarCoordenacaoAsync(
                        CodigoCoordenacaoCefga,
                        $"Novo comentário no contrato {controle.Contrato}",
                        "O solicitante (GIGOV) registrou um comentário no desembolso.",
                        link: LinkDesembolso(coControleDesembolso),
                        cancellationToken: cancellationToken);
                }
            }
        }

        public async Task ADicionarCOmentario2(ValidacaoDesembolsoRequest request)
        {
           if(request == null) throw new ArgumentNullException(nameof(request));

            var validacao = await _context.ValidacaoControleDesembolso
                .Where(v => v.CoValidacao  == request.CoValidacao
                 && v.CoControleDesembolso == request.CoControleDesembolso)
                .FirstOrDefaultAsync();

            if (validacao == null) throw new Exception($"Desembolso não encontrado!");

            if(request.ValidacaoRegistro.TipoMensagem == TipoMensagem.JUSTIFICATIVA)
            {
                validacao.Situacao = TipoSituacaoValidacao.APROVADO;
            }
        }

        public async Task ValidarDesembolso(int coControleDesembolso, ValidarDesembolsoRequest request, CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var desembolso = await _context.ControleDesembolso
                    .Include(x => x.Desembolso)
                    .Include(x => x.ValidacaoControleDesembolso)
                    .Include(x => x.Conferencias)
                    .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

                if (desembolso is null)
                    throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

                await ExecutarValidacaoDesembolso(desembolso, cancellationToken);

                await ExecutarConferenciaCamposInterno(desembolso, request.MatriculaUsuario, request.Senha, cancellationToken);

                RegistrarAuditoria(request.MatriculaUsuario, "Validar", $"Validou o desembolso {coControleDesembolso}");

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception($"Erro ao validar automaticamente o desembolso {coControleDesembolso}: {ex.Message}");
            }
        }

        public async Task ValidarTodosPendentes(ValidarDesembolsoRequest request, CancellationToken cancellationToken = default)
        {
            var desembolsos = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .Include(x => x.ValidacaoControleDesembolso)
                .Include(x => x.Conferencias)
                .Where(x => x.StatusDesembolso == TipoStatusDesembolso.PENDENTE)
                .ToListAsync(cancellationToken);

            if (desembolsos.Count == 0)
                return;

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                foreach (var desembolso in desembolsos)
                {
                    await ExecutarValidacaoDesembolso(desembolso, cancellationToken);

                    await ExecutarConferenciaCamposInterno(desembolso, request.MatriculaUsuario, request.Senha, cancellationToken);
                }

                RegistrarAuditoria(request.MatriculaUsuario, "Validar todos pendentes",
                    $"Validou em lote {desembolsos.Count} desembolso(s) pendente(s): {string.Join(", ", desembolsos.Select(d => d.CoControleDesembolso))}");

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception($"Erro ao validar desembolsos pendentes: {ex.Message}");
            }
        }

        private async Task ExecutarValidacaoDesembolso(ControleDesembolso desembolso, CancellationToken cancellationToken = default)
        {
            var resultados = await _validador.Validar(desembolso.Desembolso);

            foreach (var resultado in resultados)
            {
                var validacao = desembolso.ValidacaoControleDesembolso
                    .FirstOrDefault(x => x.CoValidacao == resultado.CoValidacao);

                if (validacao is null)
                    continue;

                validacao.Situacao = TipoSituacaoValidacao.APROVADO;
            }

            var todosAprovados = desembolso.ValidacaoControleDesembolso.Count > 0
                && desembolso.ValidacaoControleDesembolso.All(v => v.Situacao == TipoSituacaoValidacao.APROVADO);

            if (todosAprovados)
            {
                desembolso.StatusDesembolso = TipoStatusDesembolso.DESEMBOLSAR;
                desembolso.DtConclusao = DateTime.Now;
            }
            else
            {
                desembolso.StatusDesembolso = TipoStatusDesembolso.ANALISAR;
            }
        }

        public async Task<List<DesembolsoResponse>> ObterTodosDesembolsos(CancellationToken cancellationToken = default)
        {
            var desembolsos = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .Include(x => x.ValidacaoControleDesembolso)
                .ToListAsync(cancellationToken);

            return desembolsos.Select(d =>
            {
                var totalValidacoes = d.ValidacaoControleDesembolso.Count;
                var totalAprovadas = d.ValidacaoControleDesembolso.Count(v => v.Situacao == TipoSituacaoValidacao.APROVADO);

                return new DesembolsoResponse
                {
                    CoControleDesembolso = d.CoControleDesembolso,
                    Id = $"Desembolso-{d.CoControleDesembolso}",
                    NumId = d.CoDesembolso.ToString(),
                    DtSolicitado = d.Desembolso.DtSolicitado,
                    Contrato = $"{d.Desembolso.CoContratoAf}-{d.Desembolso.CoContratoAfDv}",
                    Mutuario = d.Desembolso.MutuarioFinal,
                    Gigov = d.Desembolso.CoGigov,
                    AgenteFinanceiro = d.Desembolso.AgenteFinanceiro,
                    AgentePromotor = d.Desembolso.AgentePromotor,
                    MatriculaSolicitante = d.Desembolso.MatriculaSolicitante,
                    PrimeiroDesembolso = d.Desembolso.PrimeiroDesembolso,
                    Adiantamento = d.Desembolso.TipoDesembolso == TipoDesembolso.ADIANTAMENTO,
                    UltimoDesembolso = d.Desembolso.UltimoDesembolso,
                    Recorrente = d.Desembolso.Recorrente,
                    Valor = d.Desembolso.ParticipacaoFgts,
                    ValidacoesOk = totalAprovadas,
                    ValidacoesTotal = totalValidacoes,
                    Status = d.StatusDesembolso,
                    PrazoFinal = d.DtPrazo,
                    ResponsavelAnalise = d.ResponsavelAnalise,
                    DtConclusao = d.DtConclusao,
                    DataAgendamento = d.Desembolso.Sanepar == true,
                    Sanepar = d.Desembolso.Sanepar == true,
                    ContratoAo = d.Desembolso.ContratoAo,
                    ContratoAoDv = d.Desembolso.ContratoAoDv,
                };
            }).ToList();
        }

        #region TESTADO E FUNCIONANDO

        public async Task EditarComentario(EditarComentarioRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.DeMensagem))
                throw new Exception("O comentário não pode ser vazio.");

            var registro = await _context.Mensagem
                .FirstOrDefaultAsync(x => x.CoMensagem == request.CoMensagem, cancellationToken);

            if (registro is null)
                throw new Exception($"Comentário {request.CoMensagem} não encontrado.");

            if (string.IsNullOrEmpty(registro.CoUsuario) || registro.CoUsuario != request.MatriculaSolicitante)
                throw new UnauthorizedAccessException("Apenas o autor do comentário pode editá-lo.");

            registro.DeMensagem = request.DeMensagem.Trim();

            RegistrarAuditoria(request.MatriculaSolicitante, "Editar comentário",
                $"Editou o comentário {request.CoMensagem}");

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoverComentario(int coRegistroValidacao, string matriculaSolicitante, CancellationToken cancellationToken = default)
        {
            var registro = await _context.Mensagem
                .FirstOrDefaultAsync(x => x.CoMensagem == coRegistroValidacao, cancellationToken);

            if (registro is null)
                throw new Exception($"Comentário {coRegistroValidacao} não encontrado.");

            if (string.IsNullOrEmpty(registro.CoUsuario) || registro.CoUsuario != matriculaSolicitante)
                throw new UnauthorizedAccessException("Apenas o autor do comentário pode removê-lo.");

            registro.Ativo = false;

            if (registro.TipoMensagem == TipoMensagem.PARECER)
            {
                var aindaTemParecerAtivo = await _context.Mensagem
                    .AnyAsync(x => x.CoValidacao == registro.CoValidacao
                                && x.CoControleDesembolso == registro.CoControleDesembolso
                                && x.Ativo
                                && x.TipoMensagem == TipoMensagem.PARECER
                                && x.CoMensagem != registro.CoMensagem, cancellationToken);

                if (!aindaTemParecerAtivo)
                {
                    var validacao = await _context.ValidacaoControleDesembolso
                        .FirstOrDefaultAsync(x => x.CoValidacao == registro.CoValidacao
                                                && x.CoControleDesembolso == registro.CoControleDesembolso, cancellationToken);

                    if (validacao is not null && validacao.Situacao == TipoSituacaoValidacao.APROVADO)
                    {
                        validacao.Situacao = TipoSituacaoValidacao.ANALISAR;
                    }
                }
            }

            RegistrarAuditoria(matriculaSolicitante, "Remover comentário",
                $"Removeu o comentário {coRegistroValidacao}");

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task VincularResponsavel(int coControleDesembolso, string? matriculaResponsavel, CancellationToken cancellationToken = default)
        {
            var desembolso = await _context.ControleDesembolso
                .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

            if (desembolso is null)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            if (!string.IsNullOrWhiteSpace(matriculaResponsavel))
            {
                var empregado = await _empregados.ObterPorMatricula(matriculaResponsavel);
                if (empregado is null)
                    throw new Exception($"Empregado {matriculaResponsavel} não encontrado.");
            }

            desembolso.ResponsavelAnalise = matriculaResponsavel;

            RegistrarAuditoria(matriculaResponsavel, string.IsNullOrWhiteSpace(matriculaResponsavel) ? "Remover responsável" : "Vincular responsável",
                string.IsNullOrWhiteSpace(matriculaResponsavel)
                    ? $"Removeu o responsável pela análise do desembolso {coControleDesembolso}"
                    : $"Vinculou {matriculaResponsavel} como responsável pela análise do desembolso {coControleDesembolso}");

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ValidacaoTemplateResponse>> ObterValidacoesTemplate(CancellationToken cancellationToken = default)
        {
            return await _context.Validacao
                .Where(x => !x.Desativado)
                .OrderBy(x => x.CoValidacao)
                .Select(x => new ValidacaoTemplateResponse
                {
                    CoValidacao = x.CoValidacao,
                    DeValidacao = x.DeValidacao,
                })
                .ToListAsync(cancellationToken);
        }
        #endregion

        public async Task AprovarDesembolso(int coControleDesembolso, AprovarDesembolsoRequest request, CancellationToken cancellationToken = default)
        {
            var desembolso = await _context.ControleDesembolso
                .Include(x => x.ValidacaoControleDesembolso)
                .Include(x => x.Desembolso)
                .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

            if (desembolso is null)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            var pendencias = desembolso.ValidacaoControleDesembolso
                .Where(x => x.Situacao != TipoSituacaoValidacao.APROVADO)
                .ToList();

            if (pendencias.Count > 0)
                throw new Exception(
                    $"Não é possível aprovar: {pendencias.Count} validação(ões) ainda não aprovada(s).");

            desembolso.StatusDesembolso = TipoStatusDesembolso.DESEMBOLSAR;
            desembolso.DtConclusao = DateTime.Now;
            desembolso.ResponsavelBaixa = request.MatriculaUsuario;

            RegistrarAuditoria(request.MatriculaUsuario, "Aprovar desembolso",
                $"Aprovou o desembolso {coControleDesembolso}, aguardando baixa da DRP");

            await _context.SaveChangesAsync(cancellationToken);

            await _notificacoes.EnviarParaGigovAsync(
                desembolso.Desembolso.CoGigov,
                "Desembolso aprovado",
                $"Contrato {desembolso.Desembolso.CoContratoAf}-{desembolso.Desembolso.CoContratoAfDv} foi aprovado e aguarda baixa da DRP.",
                CodigoAplicativo.Cad,
                link: LinkDesembolso(coControleDesembolso),
                cancellationToken: cancellationToken);
        }

        public async Task BaixarDRP(int coControleDesembolso, CancellationToken cancellationToken = default)
        {
            var desembolso = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

            if (desembolso is null)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            desembolso.StatusDesembolso = TipoStatusDesembolso.FINALIZADO;

            RegistrarAuditoria(null, "Baixar DRP", $"Baixou a DRP do desembolso {coControleDesembolso}");

            await _context.SaveChangesAsync(cancellationToken);

        }

        public async Task<List<RegistroDrpResponse>> ObterRegistrosDrp(CancellationToken cancellationToken = default)
        {
            var desembolsos = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .Where(x => x.StatusDesembolso == TipoStatusDesembolso.DESEMBOLSAR
                         || x.StatusDesembolso == TipoStatusDesembolso.FINALIZADO
                         || x.StatusDesembolso == TipoStatusDesembolso.REJEITADO
                         || x.StatusDesembolso == TipoStatusDesembolso.CANCELADO)
                .ToListAsync(cancellationToken);

            return desembolsos.Select(d => new RegistroDrpResponse
            {
                Id = d.CoControleDesembolso,
                Gigov = d.Desembolso.CoGigov,
                ContratoDv = $"{d.Desembolso.CoContratoAf}-{d.Desembolso.CoContratoAfDv}",
                TipoDesembolso = d.Desembolso.TipoDesembolso == TipoDesembolso.ADIANTAMENTO ? "ADIANTAMENTO" : "NORMAL",
                ValorFgts = d.Desembolso.ParticipacaoFgts,
                DataSolicitacao = d.Desembolso.DtSolicitado,
                ResponsavelBaixa = d.ResponsavelBaixa ?? string.Empty,
                Gestor = d.Desembolso.MatriculaGestor,
                ResponsavelDesembolso = d.ResponsavelDesembolso,
                Status = (int)d.StatusDesembolso,
            }).ToList();
        }

        public async Task BaixarDrpEmLote(BaixarDrpRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Ids.Count == 0)
                throw new Exception("Nenhum registro selecionado para baixa.");

            if (string.IsNullOrWhiteSpace(request.MatriculaUsuario))
                throw new Exception("Matrícula do usuário é obrigatória para confirmar a baixa.");

            var desembolsos = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .Where(x => request.Ids.Contains(x.CoControleDesembolso)
                         && x.StatusDesembolso == TipoStatusDesembolso.DESEMBOLSAR)
                .ToListAsync(cancellationToken);

            foreach (var desembolso in desembolsos)
            {
                desembolso.StatusDesembolso = TipoStatusDesembolso.FINALIZADO;
                desembolso.ResponsavelDesembolso = request.MatriculaUsuario;
            }

            RegistrarAuditoria(request.MatriculaUsuario, "Baixar DRP em lote",
                $"Baixou a DRP de {desembolsos.Count} desembolso(s): {string.Join(", ", desembolsos.Select(d => d.CoControleDesembolso))}");

            await _context.SaveChangesAsync(cancellationToken);

        }

        public async Task RejeitarDesembolso(int coControleDesembolso, RejeitarDesembolsoRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.CodigoCoordenacao))
                throw new Exception("Código da coordenação é obrigatório para rejeitar.");

            if (string.IsNullOrWhiteSpace(request.Justificativa))
                throw new Exception("A justificativa da rejeição é obrigatória.");

            var desembolso = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

            if (desembolso is null)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            desembolso.StatusDesembolso = TipoStatusDesembolso.REJEITADO;
            desembolso.DtConclusao = DateTime.Now;
            desembolso.ResponsavelBaixa = request.MatriculaUsuario;
            desembolso.Desembolso.MotivoRejeicao = request.Justificativa.Trim();

            RegistrarAuditoria(request.MatriculaUsuario, "Rejeitar desembolso",
                $"Rejeitou o desembolso {coControleDesembolso}: {request.Justificativa.Trim()}");

            await _context.SaveChangesAsync(cancellationToken);

            await _notificacoes.EnviarParaGigovAsync(
                desembolso.Desembolso.CoGigov,
                "Desembolso rejeitado",
                $"Contrato {desembolso.Desembolso.CoContratoAf}-{desembolso.Desembolso.CoContratoAfDv} foi rejeitado.",
                CodigoAplicativo.Cad,
                link: LinkDesembolso(coControleDesembolso),
                cancellationToken: cancellationToken);
        }

        public async Task CancelarDesembolso(int coControleDesembolso, CancelarDesembolsoRequest request, CancellationToken cancellationToken = default)
        {
            var desembolso = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

            if (desembolso is null)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            if (desembolso.StatusDesembolso is TipoStatusDesembolso.DESEMBOLSAR or TipoStatusDesembolso.FINALIZADO)
                throw new Exception("Não é possível cancelar um desembolso já aprovado.");

            desembolso.StatusDesembolso = TipoStatusDesembolso.CANCELADO;
            desembolso.DtConclusao = DateTime.Now;
            desembolso.MotivoCancelamento = request.Motivo.Trim();

            RegistrarAuditoria(request.MatriculaUsuario, "Cancelar desembolso",
                $"Cancelou o desembolso {coControleDesembolso}: {request.Motivo.Trim()}");

            await _context.SaveChangesAsync(cancellationToken);

            await _notificacoes.EnviarParaGigovAsync(
                desembolso.Desembolso.CoGigov,
                "Desembolso cancelado",
                $"Contrato {desembolso.Desembolso.CoContratoAf}-{desembolso.Desembolso.CoContratoAfDv} foi cancelado.",
                CodigoAplicativo.Cad,
                link: LinkDesembolso(coControleDesembolso),
                cancellationToken: cancellationToken);
        }

        public async Task AtualizarMensagemCefga(int coControleDesembolso, AtualizarMensagemCefgaRequest request, CancellationToken cancellationToken = default)
        {
            var desembolso = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

            if (desembolso is null)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            desembolso.Desembolso.MensagemCefga = request.MensagemCefga.Trim();

            RegistrarAuditoria(request.MatriculaUsuario, "Editar OBS CEFGA",
                $"Atualizou o OBS CEFGA do desembolso {coControleDesembolso}");

            await _context.SaveChangesAsync(cancellationToken);
        }

        private static readonly Dictionary<string, Func<Desembolso, (bool Ok, string? Mensagem)>> _regrasConferencia = new()
        {
            ["AgenteFinanceiro"] = d => (!string.IsNullOrWhiteSpace(d.AgenteFinanceiro), "Agente Financeiro não informado."),
            ["CnpjAgenteFinanceiro"] = d => (SoDigitos(d.CnpjAf).Length == 14, "CNPJ do Agente Financeiro inválido (precisa de 14 dígitos)."),
            ["MutuarioFinal"] = d => (!string.IsNullOrWhiteSpace(d.MutuarioFinal), "Tomador/Mutuário não informado."),
            ["CnpjMutuarioFinal"] = d => (SoDigitos(d.CnpjMutuarioFinal).Length == 14, "CNPJ do Tomador/Mutuário inválido (precisa de 14 dígitos)."),
            ["AgentePromotor"] = d => (!string.IsNullOrWhiteSpace(d.AgentePromotor), "Agente Promotor não informado."),
            ["CnpjAgentePromotor"] = d => (SoDigitos(d.CnpjAgentePromotor).Length == 14, "CNPJ do Agente Promotor inválido (precisa de 14 dígitos)."),
            ["Programa"] = d => (Enum.IsDefined(typeof(Domain.Enums.Programa), d.Programa), "Programa inválido."),
            ["CoGigov"] = d => (SoDigitos(d.CoGigov).Length == 4, "GIGOV inválido (precisa de 4 dígitos)."),
            ["MatriculaGestor"] = d => (!string.IsNullOrWhiteSpace(d.MatriculaGestor), "Matrícula do Gestor não informada."),
            ["ContratoAf"] = d => (!string.IsNullOrWhiteSpace(d.CoContratoAf) && !string.IsNullOrWhiteSpace(d.CoContratoAfDv), "Contrato AF incompleto."),
            ["DtEngenharia"] = d => (d.DtEngenharia != default, "Data de emissão da engenharia não informada."),
        };

        private static string SoDigitos(string? valor) =>
            new(string.IsNullOrEmpty(valor) ? [] : valor.Where(char.IsDigit).ToArray());

        private static string NormalizarTexto(string? valor) =>
            string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : string.Join(' ', valor.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToUpperInvariant();

        private static bool NomesConferem(string? siapf, string? cad)
        {
            var a = NormalizarTexto(siapf);
            var b = NormalizarTexto(cad);
            if (a.Length == 0 || b.Length == 0) return false;
            return a == b || a.StartsWith(b, StringComparison.Ordinal) || b.StartsWith(a, StringComparison.Ordinal);
        }

        private static readonly Dictionary<string, Func<Desembolso, CadastroGeralSiapf, (bool Ok, string? Mensagem)>> _regrasConferenciaSiapf = new()
        {
            ["SiapfContratoAf"] = (d, s) => (
                NormalizarTexto($"{s.Contrato}{s.ContratoDv}") == NormalizarTexto($"{d.CoContratoAf}{d.CoContratoAfDv}"),
                $"Contrato retornado pelo SIAPF ({s.Contrato}-{s.ContratoDv}) diverge do Contrato AF do FPD ({d.CoContratoAf}-{d.CoContratoAfDv})."),

            ["SiapfMutuarioFinal"] = (d, s) => (
                NomesConferem(s.DadosGerais.DeMutuarioFinal, d.MutuarioFinal),
                $"Tomador/Mutuário no SIAPF (\"{s.DadosGerais.DeMutuarioFinal}\") diverge do informado no FPD (\"{d.MutuarioFinal}\")."),

            ["SiapfAgentePromotor"] = (d, s) => (
                NomesConferem(s.DadosGerais.DeAgentePromotorOuParceiro, d.AgentePromotor),
                $"Agente Promotor no SIAPF (\"{s.DadosGerais.DeAgentePromotorOuParceiro}\") diverge do informado no FPD (\"{d.AgentePromotor}\")."),

            ["SiapfPrograma"] = (d, s) => (
                !string.IsNullOrWhiteSpace(s.DadosGerais.DeObjetivo)
                    && NormalizarTexto(s.DadosGerais.DeObjetivo).Contains(NormalizarTexto(d.Programa.ParaExibicao()), StringComparison.Ordinal),
                $"Objetivo do contrato no SIAPF (\"{s.DadosGerais.DeObjetivo}\") não bate com o Programa do FPD (\"{d.Programa.ParaExibicao()}\")."),
        };

        private async Task<List<ConferenciaControleDesembolso>> ExecutarConferenciaCamposInterno(
            ControleDesembolso desembolso,
            string? matriculaUsuario = null,
            string? senha = null,
            CancellationToken cancellationToken = default)
        {
            var camposAtivos = await _context.CampoConferencia
                .Where(x => !x.Desativado)
                .OrderBy(x => x.CoCampo)
                .ToListAsync(cancellationToken);

            _context.ConferenciaControleDesembolso.RemoveRange(desembolso.Conferencias);

            CadastroGeralSiapf? cadastroSiapf = null;
            string? erroConsultaSiapf = null;

            var precisaConsultarSiapf = camposAtivos.Any(c => _regrasConferenciaSiapf.ContainsKey(c.Chave));

            if (precisaConsultarSiapf)
            {
                try
                {
                    cadastroSiapf = await _siapf.ConsultarCadastroGeralAsync(
                        desembolso.Desembolso.CoContratoAf,
                        desembolso.Desembolso.CoContratoAfDv,
                        matriculaUsuario ?? string.Empty,
                        senha ?? string.Empty,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    erroConsultaSiapf = $"Falha ao consultar o SIAPF: {ex.Message}";
                }
            }

            var novasConferencias = new List<ConferenciaControleDesembolso>();

            foreach (var campo in camposAtivos)
            {
                TipoSituacaoConferencia situacao;
                string? mensagem;

                if (_regrasConferenciaSiapf.TryGetValue(campo.Chave, out var regraSiapf))
                {
                    if (cadastroSiapf is not null)
                    {
                        var (ok, msgFalha) = regraSiapf(desembolso.Desembolso, cadastroSiapf);
                        situacao = ok ? TipoSituacaoConferencia.OK : TipoSituacaoConferencia.ERRO;
                        mensagem = ok ? null : msgFalha;
                    }
                    else
                    {
                        situacao = TipoSituacaoConferencia.PENDENTE;
                        mensagem = erroConsultaSiapf ?? "Consulta ao SIAPF não realizada.";
                    }
                }
                else if (_regrasConferencia.TryGetValue(campo.Chave, out var regra))
                {
                    var (ok, msgFalha) = regra(desembolso.Desembolso);
                    situacao = ok ? TipoSituacaoConferencia.OK : TipoSituacaoConferencia.ERRO;
                    mensagem = ok ? null : msgFalha;
                }
                else
                {
                    situacao = TipoSituacaoConferencia.PENDENTE;
                    mensagem = "Verificação ainda não implementada pro sistema.";
                }

                novasConferencias.Add(new ConferenciaControleDesembolso
                {
                    CoControleDesembolso = desembolso.CoControleDesembolso,
                    CoCampo = campo.CoCampo,
                    DeCampo = campo.DeCampo,
                    Situacao = situacao,
                    Mensagem = mensagem,
                    DtConferencia = DateTime.Now,
                });
            }

            _context.ConferenciaControleDesembolso.AddRange(novasConferencias);
            desembolso.DtUltimaConferencia = DateTime.Now;

            return novasConferencias;
        }

        public async Task<List<ConferenciaCampoResponse>> ExecutarConferenciaCampos(int coControleDesembolso, CancellationToken cancellationToken = default)
        {
            var desembolso = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .Include(x => x.Conferencias)
                .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

            if (desembolso is null)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            var novasConferencias = await ExecutarConferenciaCamposInterno(desembolso, cancellationToken: cancellationToken);

            RegistrarAuditoria(null, "Executar conferência de campos",
                $"Rodou a conferência de campos do desembolso {coControleDesembolso}: {novasConferencias.Count} item(ns).");

            await _context.SaveChangesAsync(cancellationToken);

            return novasConferencias
                .Select(c => new ConferenciaCampoResponse
                {
                    CoCampo = c.CoCampo,
                    DeCampo = c.DeCampo,
                    Situacao = c.Situacao.ToString(),
                    Mensagem = c.Mensagem,
                })
                .ToList();
        }

        public async Task RejeitarDesembolsoTeste(int idDesembolso)
        {
            var desembolso = await _context.ControleDesembolso
            .FirstOrDefaultAsync(d => d.CoControleDesembolso == idDesembolso);
            if (desembolso is null)
                throw new Exception($"Desembolso {idDesembolso} não encontrado.");
        }

        private const int PrazoDesembolsoDiasUteis = 2;

        private static DateTime AdicionarDiasUteis(DateTime dataBase)
        {
            var data = dataBase;
            var adicionados = 0;
            while (adicionados < PrazoDesembolsoDiasUteis)
            {
                data = data.AddDays(1);
                if (data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday)
                    adicionados++;
            }
            return data;
        }
    }
}
