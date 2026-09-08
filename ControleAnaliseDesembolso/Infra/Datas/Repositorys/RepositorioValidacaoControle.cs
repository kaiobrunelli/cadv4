using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using Microsoft.EntityFrameworkCore;

namespace ControleAnaliseDesembolso.Infra.Datas.Repositorys
{
    public class RepositorioValidacaoControle : RepositorioBase<ValidacaoControleDesembolso>, IRepositorioValidacaoControle
    {
        private readonly ControleAnaliseDesembolsoContext _context;

        public RepositorioValidacaoControle(ControleAnaliseDesembolsoContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ValidacaoControleDesembolso?> BuscarValidacao(int CoValidacao, int CoControleDesembolso, CancellationToken cancellationToken)
        {
            // OrderByDescending por DtValidacao em vez de depender de PK única: itens de
            // conferência podem ter mais de uma linha (histórico); pega sempre a mais recente.
            return await _context.ValidacaoControleDesembolso
                    .Where(x => x.CoValidacao == CoValidacao && x.CoControleDesembolso == CoControleDesembolso)
                    .OrderByDescending(x => x.DtValidacao)
                    .FirstOrDefaultAsync(cancellationToken);
        }

    }
}
