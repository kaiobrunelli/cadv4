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

        // NOTA: o .Include(x => x.Conferencias) abaixo NÃO existe no projeto de
        // referência (XP Metodo nvoo/ControleAnaliseDesembolsoRepositorio) — lá
        // ControleDesembolso ainda não tinha a conferência de campos
        // (CAD_TB005/TB006, adicionada depois). Sem esse Include, ObterDetalheDesembolso
        // ficaria sem os dados de ConferenciaCampos que a tela já usa hoje.
        // Precisei acrescentar pra não regredir uma funcionalidade que já existe no CAD.
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
