using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Domain.Entitys;

namespace ControleAnaliseDesembolso.Application.Interface
{
    public interface IValidadorDesembolsoService
    {
        Task<List<ResultadoValidacaoAutomatica>> Validar(Desembolso fpd);
    }
}
