using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Application.Interface;
using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Enums;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using ControleAnaliseDesembolso.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PlataformaNotificacao.Application.Interface;
using PlataformaNotificacao.Domain.Enum;
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

        private const string CodigoCoordenacaoCefga = "CEFGA06";

        public ControleAnaliseDesembolsoService(
            ControleAnaliseDesembolsoContext context,
            IValidadorDesembolsoService validador,
            IEmpregadoCADService empregados,
            INotificacaoService notificacoes,
            IHttpContextAccessor httpContextAccessor,
            UtilitarioMapperServicecopy mapperCopy)
        {
            _context = context;
            _validador = validador;
            _empregados = empregados;
            _notificacoes = notificacoes;
            _httpContextAccessor = httpContextAccessor;
            _mapperCopy = mapperCopy;
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
            var fpd = await _context.Desembolso
                .Include(x => x.ControleDesembolso)
                .FirstOrDefaultAsync(x => x.CoDesembolso == coFpd, cancellationToken);

            var response = fpd.ControleDesembolso.ResponsavelAnalise;
            return response;
        }

        public async Task<List<ComentarioValidacaoResponse>> ObterComentarios(int coControleDesembolso, CancellationToken cancellationToken = default)
        {
            var existe = await _context.ControleDesembolso.AnyAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);
            if (!existe)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            return await _context.Mensagem
                .Where(x => x.CoControleDesembolso == coControleDesembolso && x.Ativo)
                .OrderBy(x => x.DtCriacao)
                .Select(x => new ComentarioValidacaoResponse
                {
                    CoMensagem = x.CoMensagem,
                    CoValidacao = x.CoValidacao,
                    Texto = x.DeMensagem,
                    TipoMensagem = x.TipoMensagem,
                    MatriculaAutor = x.CoUsuario,
                    NomeAutor = x.DeUsuario,
                    UnidadeAutor = x.UnidadeUsuario,
                    Sigla = x.UnidadeUsuario == 7175 ? "CEFGA" : "GIGOV",
                    DtCriacao = x.DtCriacao,
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<DesembolsoDetalheResponse> ObterDetalheDesembolso(int coControleDesembolso, CancellationToken cancellationToken = default)
        {
            var desembolso = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .Include(x => x.ValidacaoControleDesembolso)
                    .ThenInclude(x => x.Validacao)
                .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

            if (desembolso is null)
                throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

            var comentarios = await _context.Mensagem
                .Where(x => x.CoControleDesembolso == coControleDesembolso && x.Ativo)
                .OrderBy(x => x.DtCriacao)
                .Select(x => new ComentarioValidacaoResponse
                {
                    CoMensagem = x.CoMensagem,
                    CoValidacao = x.CoValidacao,
                    Texto = x.DeMensagem,
                    TipoMensagem = x.TipoMensagem,
                    MatriculaAutor = x.CoUsuario,
                    NomeAutor = x.DeUsuario,
                    UnidadeAutor = x.UnidadeUsuario,
                    Sigla = x.UnidadeUsuario == 7175 ? "CEFGA" : "GIGOV",
                    DtCriacao = x.DtCriacao,
                })
                .ToListAsync(cancellationToken);

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
                Mensagem = fpd.Mensagem,
                MotivoRejeicao = fpd.MotivoRejeicao,
                CoContratoAf = fpd.CoContratoAf,
                CoContratoAfDv = fpd.CoContratoAfDv,
                ContratoAo = fpd.ContratoAo,
                CoGigov = fpd.CoGigov,
                MutuarioFinal = fpd.MutuarioFinal,
                CnpjMutuarioFinal = fpd.CnpjMutuarioFinal,
                AgenteFinanceiro = fpd.AgenteFinanceiro,
                AgentePromotor = fpd.AgentePromotor,
                Programa = fpd.Programa.ToString(),
                TipoDesembolso = fpd.TipoDesembolso.ToString(),
                PrimeiroDesembolso = fpd.PrimeiroDesembolso,
                UltimoDesembolso = fpd.UltimoDesembolso,
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
                SaldoADesembolsar = fpd.SaldoADesembolsar,
                Excepcionalizado = fpd.Excepcionalizado,
                ContrapartidaAtual = fpd.ContrapartidaAtual,
                Integralizado = fpd.Integralizado,
                SaldoIntegralizar = fpd.SaldoIntegralizar,
                ContrapartidaAlterada = fpd.ContrapartidaAlterada,
                //Amortizacao = fpd.Amortizacao,
                Sanepar = fpd.Sanepar,
                RetornoParcial = fpd.RetornoParcial,
                PlacaLocal = fpd.PlacaLocal,
                LicensaInstalacao = fpd.LicensaInstalacao,
                LicensaOperacao = fpd.LicensaOperacao,
                Funcionalidade = fpd.Funcionalidade,
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
                };

                _context.Desembolso.Add(fpd);
                await _context.SaveChangesAsync(cancellationToken);

                coControleDesembolso = fpd.ControleDesembolso.CoControleDesembolso;

                var validacoesModelo = await _context.Validacao
                    .Where(x => !x.Desativado)
                    .OrderBy(x => x.CoValidacao)
                    .ToListAsync(cancellationToken);

                var registrosPorValidacao = request.ValidacoesDesembolsoRequest
                    .Where(x => x.ValidacaoRegistro is not null &&
                                !string.IsNullOrWhiteSpace(x.ValidacaoRegistro.DeMensagem))
                    .GroupBy(x => x.CoValidacao)
                    .ToDictionary(x => x.Key, x => x.ToList());

                foreach (var modelo in validacoesModelo)
                {
                    registrosPorValidacao.TryGetValue(modelo.CoValidacao, out var registrosDoItem);

                    var temJustificativa = registrosDoItem?.Any(
                        r => r.ValidacaoRegistro.TipoMensagem == TipoMensagem.JUSTIFICATIVA) ?? false;

                    var validacaoDesembolso = new ValidacaoControleDesembolso
                    {
                        CoValidacao = modelo.CoValidacao,
                        CoControleDesembolso = coControleDesembolso,
                        DeValidacao = modelo.DeValidacao,
                        CampoVinculado = null,
                        Situacao = temJustificativa ? TipoSituacaoValidacao.APROVADO : TipoSituacaoValidacao.ANALISAR,
                    };

                    _context.ValidacaoControleDesembolso.Add(validacaoDesembolso);

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

                        _context.Mensagem.Add(registro);
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
                fpd = await _context.Desembolso
                    .Include(x => x.ControleDesembolso)
                        .ThenInclude(d => d.ValidacaoControleDesembolso)
                    .FirstOrDefaultAsync(x => x.CoDesembolso == coFpd, cancellationToken);

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
                    _context.ValidacaoControleDesembolso.Add(new ValidacaoControleDesembolso
                    {
                        CoValidacao = modelo.CoValidacao,
                        CoControleDesembolso = fpd.ControleDesembolso.CoControleDesembolso,
                        DeValidacao = modelo.DeValidacao,
                        Situacao = TipoSituacaoValidacao.ANALISAR,
                    });
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
                        _context.Mensagem.Add(registro);
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
            fpd.SolicitadoVi = request.SolicitadoVi;
            fpd.GlossadoVi = request.GlossadoVi;
            fpd.AceitoVi = request.AceitoVi;
            fpd.ParticipacaoFgts = request.ParticipacaoFgts;
            fpd.Contrapartida = request.Contrapartida;
            fpd.ValorEmprestimo = request.ValorEmprestimo;
            fpd.Desembolsado = request.Desembolsado;
            fpd.SaldoADesembolsar = request.SaldoADesembolsar;
            fpd.Excepcionalizado = request.Excepcionalizado;
            fpd.ContrapartidaAtual = request.ContrapartidaAtual;
            fpd.Integralizado = request.Integralizado;
            fpd.SaldoIntegralizar = request.SaldoIntegralizar;
            fpd.ContrapartidaAlterada = request.ContrapartidaAlterada;
            //fpd.Amortizacao = request.Amortizacao;
            fpd.Sanepar = request.Sanepar;
            fpd.Mensagem = request.Mensagem;
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

            var validacao = await _context.ValidacaoControleDesembolso
                .FirstOrDefaultAsync(x => x.CoValidacao == request.CoValidacao
                                        && x.CoControleDesembolso == request.CoControleDesembolso, cancellationToken);

            if (validacao is null)
                throw new Exception($"Validação {request.CoValidacao} do desembolso {request.CoControleDesembolso} não encontrada.");


            if (request.ValidacaoRegistro.TipoMensagem == TipoMensagem.JUSTIFICATIVA)
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

            await _context.Mensagem.AddAsync(registro, cancellationToken);

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
                // Se precisarmos, mudamos a regra de notificação para ser responsável e gestor
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

        public async Task ValidarDesembolso(int coControleDesembolso, CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var desembolso = await _context.ControleDesembolso
                    .Include(x => x.Desembolso)
                    .Include(x => x.ValidacaoControleDesembolso)
                    .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);

                if (desembolso is null)
                    throw new Exception($"Desembolso {coControleDesembolso} não encontrado.");

                // Método para atualizar a tabela de desembolso com o Contrato AO
                // TODO: implementar a busca real assim que o acesso ao sistema externo estiver disponível.
                // var contratoAo = await _sistemaExterno.ConsultarContratoAoAsync(
                //     desembolso.Desembolso.CoContratoAf, desembolso.Desembolso.CoContratoAfDv, cancellationToken);
                //
                // -- consulta equivalente, caso a origem seja direto no banco do sistema externo:
                // -- SELECT CONTRATO_AO FROM <TABELA_DO_SISTEMA_EXTERNO>
                // -- WHERE CO_CONTRATO_AF = @CoContratoAf AND CO_CONTRATO_AF_DV = @CoContratoAfDv
                //
                // if (!string.IsNullOrWhiteSpace(contratoAo))
                //     desembolso.Desembolso.ContratoAo = contratoAo;

                await ExecutarValidacaoDesembolso(desembolso);

                RegistrarAuditoria(null, "Validar", $"Validou o desembolso {coControleDesembolso}");

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception($"Erro ao validar automaticamente o desembolso {coControleDesembolso}: {ex.Message}");
            }
        }

        public async Task ValidarTodosPendentes(CancellationToken cancellationToken = default)
        {
            var desembolsos = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .Include(x => x.ValidacaoControleDesembolso)
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
                    await ExecutarValidacaoDesembolso(desembolso);
                }

                RegistrarAuditoria(null, "Validar todos pendentes",
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

        private async Task ExecutarValidacaoDesembolso(ControleDesembolso desembolso)
        {
            var resultados = await _validador.Validar(desembolso.Desembolso);

            foreach (var resultado in resultados)
            {
                var validacao = desembolso.ValidacaoControleDesembolso
                    .FirstOrDefault(x => x.CoValidacao == resultado.CoValidacao);

                if (validacao is null)
                    continue;

                // aqui será a lógica do SIAPF para validar item a item
                validacao.Situacao = TipoSituacaoValidacao.APROVADO;

            }

            // validacao.Situacao acima sempre vira Aprovado (stub — aqui será a
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
                    Valor = d.Desembolso.ParticipacaoFgts,
                    ValidacoesOk = totalAprovadas,
                    ValidacoesTotal = totalValidacoes,
                    Status = d.StatusDesembolso,
                    PrazoFinal = d.DtPrazo,
                    ResponsavelAnalise = d.ResponsavelAnalise,
                    DtConclusao = d.DtConclusao,
                    DataAgendamento = d.Desembolso.Sanepar == true,
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

            if (registro.TipoMensagem == TipoMensagem.JUSTIFICATIVA)
            {
                var aindaTemJustificativaAtiva = await _context.Mensagem
                    .AnyAsync(x => x.CoValidacao == registro.CoValidacao
                                && x.CoControleDesembolso == registro.CoControleDesembolso
                                && x.Ativo
                                && x.TipoMensagem == TipoMensagem.JUSTIFICATIVA
                                && x.CoMensagem != registro.CoMensagem, cancellationToken);

                if (!aindaTemJustificativaAtiva)
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

            desembolso.StatusDesembolso = TipoStatusDesembolso.FINALIZAR;

            RegistrarAuditoria(null, "Baixar DRP", $"Baixou a DRP do desembolso {coControleDesembolso}");

            await _context.SaveChangesAsync(cancellationToken);

        }

        public async Task<List<RegistroDrpResponse>> ObterRegistrosDrp(CancellationToken cancellationToken = default)
        {
            var desembolsos = await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .Where(x => x.StatusDesembolso == TipoStatusDesembolso.DESEMBOLSAR
                         || x.StatusDesembolso == TipoStatusDesembolso.FINALIZAR
                         || x.StatusDesembolso == TipoStatusDesembolso.NEGAR)
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
                desembolso.StatusDesembolso = TipoStatusDesembolso.FINALIZAR;
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

            desembolso.StatusDesembolso = TipoStatusDesembolso.NEGAR;
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
