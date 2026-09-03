using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using Microsoft.EntityFrameworkCore;

namespace ControleAnaliseDesembolso.Infra.Datas.Repositorys
{
    public class RepositorioControle : RepositorioBase<ControleDesembolso>, IRepositorioControle
    {

        private readonly ControleAnaliseDesembolsoContext _context;

        public RepositorioControle(ControleAnaliseDesembolsoContext context) : base(context)
        {

            _context = context;

        }

        public async Task<bool> ExisteControleDesembolso(int coDesembolso, CancellationToken cancellationToken)
        {
            return await _context.ControleDesembolso.AnyAsync(x => x.CoControleDesembolso == coDesembolso, cancellationToken);
        }

        public async Task<ControleDesembolso?> ObterControleDesembolsoCompleto(int coControleDesembolso, CancellationToken cancellationToken)
        {
            return await _context.ControleDesembolso
                .Include(x => x.Desembolso)
                .Include(x => x.ValidacaoControleDesembolso)
                    .ThenInclude(x => x.Validacao)
                .Include(x => x.Conferencias)
                .FirstOrDefaultAsync(x => x.CoControleDesembolso == coControleDesembolso, cancellationToken);
        }
    }

}
