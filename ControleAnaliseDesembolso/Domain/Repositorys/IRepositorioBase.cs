using System.Linq.Expressions;

namespace ControleAnaliseDesembolso.Domain.Repositorys
{
    public interface IRepositorioBase<TEntity> where TEntity : class
    {
        Task Adicionar(TEntity obj, CancellationToken cancellationToken = default);
        Task Atualizar(TEntity obj, CancellationToken cancellationToken = default);
        Task AdicionarSemSalvar(TEntity obj, CancellationToken cancellationToken = default);
        Task Remove(TEntity obj, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> ObterTodos(CancellationToken cancellationToken = default);
        Task<TEntity?> ObterContrato<TKey>(Expression<Func<TEntity, bool>> filtro, Expression<Func<TEntity, TKey>> ordenacao, CancellationToken cancellationToken = default);
    }
}
