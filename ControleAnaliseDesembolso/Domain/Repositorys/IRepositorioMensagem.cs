using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Domain.Entitys;

namespace ControleAnaliseDesembolso.Domain.Repositorys
{
    public interface IRepositorioMensagem : IRepositorioBase<Mensagem>
    {
        Task<List<ComentarioValidacaoResponse>> ObterComentario(int coDesembolso, CancellationToken cancellationToken);

    }
}
