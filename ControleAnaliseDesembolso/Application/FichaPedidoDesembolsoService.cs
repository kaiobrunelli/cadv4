using ControleAnaliseDesembolso.Application.Dtos.Request;
using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Application.Interface;
using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Enums;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using ControleAnaliseDesembolso.Infra.Datas.Repositorys;

namespace ControleAnaliseDesembolso.Application
{
    public class FichaPedidoDesembolsoService : IFichaPedidoDesembolsoService
    {
        private readonly IRepositorioFichaPedidoDesembolso _repositorioFpd;

        public FichaPedidoDesembolsoService(ControleAnaliseDesembolsoContext context)
        {
            _repositorioFpd = new RepositorioFichaPedidoDesembolso(context);
        }

        private static Desembolso MapearParaFicha(PedidoDesembolsoRequest request)
        {
            return new Desembolso
            {
                MatriculaSolicitante = request.MatriculaSolicitante,
                CoGigov = request.CoGigov,
                MatriculaGestor = request.MatriculaGestor,
                NuDesembolso = request.NuDesembolso,
                CoContratoAf = request.CoContratoAf,
                CoContratoAfDv = request.CoContratoAfDv,
                PrimeiroDesembolso = request.PrimeiroDesembolso,
                AgenteFinanceiro = request.AgenteFinanceiro,
                CnpjAf = request.CnpjAf,
                MutuarioFinal = request.MutuarioFinal,
                CnpjMutuarioFinal = request.CnpjMutuarioFinal,
                AgenteTecnicoOperador = request.AgenteTecnicoOperador,
                CnpjAgenteTecnicoOperador = request.CnpjAgenteTecnicoOperador,
                AgentePromotor = request.AgentePromotor,
                CnpjAgentePromotor = request.CnpjAgentePromotor,
                Programa = (Programa)request.Programa,
                UltimoDesembolso = request.UltimoDesembolso,
                Funcionalidade = request.Funcionalidade,
                Concluido = request.Concluido,
                DtEngenharia = request.DtEngenharia,
                SituacaoObra = (TipoSituacaoObra)request.SituacaoObra,
                DtSocioAmbiental = request.DtSocioAmbiental,
                PercentualObra = request.PercentualObra,
                TipoDesembolso = (TipoDesembolso)request.TipoDesembolso,
                RetornoParcial = request.RetornoParcial,
                PlacaLocal = request.PlacaLocal,
                LicensaInstalacao = request.LicensaInstalacao,
                LicensaOperacao = request.LicensaOperacao,
                CndValido = request.CndValido,
                CrpValido = request.CrpValido,
                SolicitadoVi = request.SolicitadoVi,
                GlossadoVi = request.GlossadoVi,
                AceitoVi = request.AceitoVi,
                ParticipacaoFgts = request.ParticipacaoFgts,
                Contrapartida = request.Contrapartida,
                ValorEmprestimo = request.ValorEmprestimo,
                Desembolsado = request.Desembolsado,
                SaldoADesembolsar = request.SaldoADesembolsar,
                Excepcionalizado = request.Excepcionalizado,
                ContrapartidaAtual = request.ContrapartidaAtual,
                Integralizado = request.Integralizado,
                SaldoIntegralizar = request.SaldoIntegralizar,
                ContrapartidaAlterada = request.ContrapartidaAlterada,
                //Amortizacao = request.Amortizacao,
                Sanepar = request.Sanepar,
                Mensagem = request.Mensagem,
            };
        }

        public async Task SalvarFpd(PedidoDesembolsoRequest Fpd, CancellationToken cancellationToken = default)
        {
            try
            {
                var fpd = MapearParaFicha(Fpd);
                fpd.DtSolicitado = DateTime.Now;

                await _repositorioFpd.Adicionar(fpd, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao tentar salvar a Ficha de Pedido De Desembolso: {Fpd}. {ex.Message}");
            }
        }

        public async Task<PedidoConsultaContratoAfResponse> SolicitarDadosFPD(PedidoConsultaContratoAfRequest Pedido, CancellationToken cancellationToken = default)
        {
            var FpdAnterior = await _repositorioFpd.ObterContrato(x =>
                                        x.CoContratoAf == Pedido.CoContratoAf &&
                                        x.CoContratoAfDv == Pedido.CoContratoAfDv,
                                        x => x.CoDesembolso,
                                        cancellationToken);

            if (FpdAnterior is null)
            {
                throw new Exception($"Contrato: {Pedido.CoContratoAf}-{Pedido.CoContratoAfDv} não encontrado.");
            }

            return new PedidoConsultaContratoAfResponse
            {
                CoContratoAf = FpdAnterior.CoContratoAf,
                CoContratoAfDv = FpdAnterior.CoContratoAfDv,
                NuFpd = FpdAnterior.NuDesembolso,
                CoGigov = FpdAnterior.CoGigov,
                MatriculaGestor = FpdAnterior.MatriculaGestor,
                AgenteFinanceiro = FpdAnterior.AgenteFinanceiro,
                CnpjAf = FpdAnterior.CnpjAf,
                MutuarioFinal = FpdAnterior.MutuarioFinal,
                CnpjMutuarioFinal = FpdAnterior.CnpjMutuarioFinal,
                AgenteTecnicoOperador = FpdAnterior.AgenteTecnicoOperador,
                CnpjAgenteTecnicoOperador = FpdAnterior.CnpjAgenteTecnicoOperador,
                AgentePromotor = FpdAnterior.AgentePromotor,
                CnpjAgentePromotor = FpdAnterior.CnpjAgentePromotor,
                Programa = FpdAnterior.Programa.ToString(),
                RetornoParcial = FpdAnterior.RetornoParcial ?? false,
                DtEngenharia = FpdAnterior.DtEngenharia,
                SituacaoObra = FpdAnterior.SituacaoObra?.ToString(),
                DtSocioAmbiental = FpdAnterior.DtSocioAmbiental,
                PercentualObra = FpdAnterior.PercentualObra,
                ValorEmprestimo = FpdAnterior.ValorEmprestimo,
                Desembolsado = FpdAnterior.Desembolsado,
                ContrapartidaAtual = FpdAnterior.ContrapartidaAtual,
                Integralizado = FpdAnterior.Integralizado,
            };
        }
    }
}
