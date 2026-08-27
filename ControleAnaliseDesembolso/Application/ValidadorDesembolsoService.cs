using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Application.Interface;
using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Application
{
    public class ValidadorDesembolsoService : IValidadorDesembolsoService
    {
        private static readonly List<Func<Desembolso, ResultadoValidacaoAutomatica>> _regras =
        [
            fpd => RegraBooleana(coValidacao: 1, valor: fpd.LicensaOperacao, nomeCampo: "Licença de operação"),
            fpd => RegraBooleana(coValidacao: 2, valor: fpd.LicensaInstalacao, nomeCampo: "Licença de instalação"),
            //fpd => RegraBooleana(coValidacao: 6, valor: fpd.Amortizacao, nomeCampo: "Amortização"),
            fpd => RegraBooleana(coValidacao: 7, valor: fpd.RetornoParcial, nomeCampo: "Retorno parcial"),
            fpd => RegraBooleana(coValidacao: 8, valor: fpd.PlacaLocal, nomeCampo: "Placa local"),
            fpd => RegraBooleana(coValidacao: 9, valor: fpd.Excepcionalizado, nomeCampo: "Excepcionalização"),
            fpd => RegraBooleana(coValidacao: 1005, valor: fpd.ContrapartidaAlterada, nomeCampo: "CP alterada"),
        ];

        public Task<List<ResultadoValidacaoAutomatica>> Validar(Desembolso fpd) =>
            Task.FromResult(_regras.Select(regra => regra(fpd)).ToList());

        private static ResultadoValidacaoAutomatica RegraBooleana(int coValidacao, bool? valor, string nomeCampo)
        {
            var aprovado = valor == true;   
            return new ResultadoValidacaoAutomatica
            {
                CoValidacao = coValidacao,
                Aprovado = aprovado,
                Mensagem = aprovado ? $"{nomeCampo}: OK." : $"{nomeCampo}: pendente ou não informado.",
            };
        }
    }
}
