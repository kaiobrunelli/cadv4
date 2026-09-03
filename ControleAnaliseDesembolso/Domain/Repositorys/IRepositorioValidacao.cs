using ControleAnaliseDesembolso.Domain.Entitys;

namespace ControleAnaliseDesembolso.Domain.Repositorys
{
    public interface IRepositorioValidacao : IRepositorioBase<Validacao>
    {
        Task<List<Validacao>?> TrazerValidacao(CancellationToken cancellationToken);

    }
}
