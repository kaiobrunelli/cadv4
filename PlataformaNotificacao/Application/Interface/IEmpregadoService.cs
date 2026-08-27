using PlataformaNotificacao.Application.Interface;
using PlataformaNotificacao.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaNotificacao.Application.Interface
{
    public interface IEmpregadoService
    {
        List<string> ObterMatriculasTodos();
        Task<List<string>> ObterTodasMatriculas();
        Task<List<string>> ObterMatriculasPorCoordenacao(string codigoCoordenacao);
        List<string> ObterMatriculasPorModulo(string modulo);
        List<string> FiltrarMatriculasValidas(IEnumerable<string> matriculas);

        Task<List<string>> ObterMatriculasGigov();

        Task<List<string>> ObterMatriculasGigovPorNumero(string codigoGigov);

        Task<string?> ObterGestorAtivoOuEventualAsync(string codigoCoordenacao);
    }
}



