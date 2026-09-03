using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using Microsoft.EntityFrameworkCore;

namespace ControleAnaliseDesembolso.Infra.Datas.Repositorys
{
    public class RepositorioDesembolso : RepositorioBase<Desembolso>, IRepositorioDesembolso
    {

        private readonly ControleAnaliseDesembolsoContext _context;

        public RepositorioDesembolso(ControleAnaliseDesembolsoContext context) : base(context)
        {
            _context = context;

        }

        public async Task<Desembolso?> ObterDesembolso(int coDesembolso, CancellationToken cancellationToken = default)
        {
            return await _context.Desembolso
                .Include(x => x.ControleDesembolso)
                .ThenInclude(d => d.ValidacaoControleDesembolso)
                .FirstOrDefaultAsync(x => x.CoDesembolso == coDesembolso, cancellationToken);
        }

    }
}
