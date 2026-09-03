using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using Microsoft.EntityFrameworkCore;

namespace ControleAnaliseDesembolso.Infra.Datas.Repositorys
{
    public class RepositorioValidacao : RepositorioBase<Validacao>, IRepositorioValidacao
    {
        private readonly ControleAnaliseDesembolsoContext _context;

        public RepositorioValidacao(ControleAnaliseDesembolsoContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Validacao>?> TrazerValidacao(CancellationToken cancellationToken)
        {
            return await _context.Validacao
                    .Where(x => !x.Desativado)
                    .OrderBy(x => x.CoValidacao)
                    .ToListAsync(cancellationToken);
        }

    }
}
