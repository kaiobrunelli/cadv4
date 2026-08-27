using System.Linq.Expressions;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using Microsoft.EntityFrameworkCore;

namespace ControleAnaliseDesembolso.Infra.Datas.Repositorys
{
    public class RepositorioBase<TEntity>(
        ControleAnaliseDesembolsoContext context
    ) : IRepositorioBase<TEntity> where TEntity : class
    {
        private readonly ControleAnaliseDesembolsoContext _context = context;

        public async Task Adicionar(TEntity obj, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<TEntity>().Add(obj);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                throw new Exception($"Erro ao tentar salvar o objeto: {obj}");
            }
        }

        public async Task Atualizar(TEntity obj, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Entry(obj).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                throw new Exception($"Contrato não encontrado.");
            }
        }

        public async Task<TEntity?> ObterContrato<TKey>(Expression<Func<TEntity, bool>> filtro, Expression<Func<TEntity, TKey>> ordenacao, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Set<TEntity>()
                    .Where(filtro)
                    .OrderByDescending(ordenacao)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<IEnumerable<TEntity>> ObterTodos(CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public async Task Remove(TEntity obj, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<TEntity>().Remove(obj);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                throw new Exception($"Contrato não encontrado.");
            }
        }
    }
}
