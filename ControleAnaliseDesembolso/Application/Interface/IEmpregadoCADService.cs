using ControleAnaliseDesembolso.Domain.Entitys;

namespace ControleAnaliseDesembolso.Application.Interface
{
    public interface IEmpregadoCADService
    {
        Task<List<Empregado>> ObterTodos();
        Task<Empregado?> ObterPorMatricula(string matricula);

        Task<List<Empregado>> ObterEmpregadosPorCoordenacao(string codigoCoordenacao);

        Task<List<string>> ObterCodigosGigovPorMatricula(string matricula);
    }
}
