using ControleAnaliseDesembolso.Domain.Entitys;

namespace ControleAnaliseDesembolso.Domain.Repositorys
{
    public interface IRepositorioControle : IRepositorioBase<ControleDesembolso>
    {
        public Task<bool> ExisteControleDesembolso(int coDesembolso, CancellationToken cancellationToken);
        public Task<ControleDesembolso?> ObterControleDesembolsoCompleto(int coControleDesembolso, CancellationToken cancellationToken);

    }
}
