using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Enums;
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
            // Só o checklist manual/automático — itens de conferência (ConferenciaLocal/
            // ConferenciaSiapf) ganham a primeira linha só quando a conferência roda
            // (ver ExecutarConferenciaCamposInterno), não na criação/reenvio da FPD.
            return await _context.Validacao
                    .Where(x => !x.Desativado
                        && (x.Origem == TipoOrigemValidacao.Manual || x.Origem == TipoOrigemValidacao.AutomaticaCampo))
                    .OrderBy(x => x.CoValidacao)
                    .ToListAsync(cancellationToken);
        }

    }
}
