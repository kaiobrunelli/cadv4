using ControleAnaliseDesembolso.Domain.Entitys;

namespace ControleAnaliseDesembolso.Domain.Repositorys
{
    public interface IRepositorioDesembolso : IRepositorioBase<Desembolso>
    {
        Task<Desembolso?> ObterDesembolso(int coDesembolso, CancellationToken cancellationToken);
    }
}
