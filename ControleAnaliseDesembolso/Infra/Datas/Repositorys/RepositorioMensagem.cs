using ControleAnaliseDesembolso.Application.Dtos.Response;
using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using Microsoft.EntityFrameworkCore;

namespace ControleAnaliseDesembolso.Infra.Datas.Repositorys
{
    public class RepositorioMensagem : RepositorioBase<Mensagem>, IRepositorioMensagem
    {

        private readonly ControleAnaliseDesembolsoContext _context;

        public RepositorioMensagem(ControleAnaliseDesembolsoContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ComentarioValidacaoResponse>> ObterComentario(int coDesembolso, CancellationToken cancellationToken)
        {
            var mensagem = await _context.Mensagem
                .Where(x => x.CoControleDesembolso == coDesembolso && x.Ativo)
                .OrderBy(x => x.DtCriacao)
                .Select(x => new ComentarioValidacaoResponse
                {
                    CoMensagem = x.CoMensagem,
                    CoValidacao = x.CoValidacao,
                    Texto = x.DeMensagem,
                    TipoMensagem = x.TipoMensagem,
                    MatriculaAutor = x.CoUsuario,
                    NomeAutor = x.DeUsuario,
                    UnidadeAutor = x.UnidadeUsuario,
                    Sigla = x.UnidadeUsuario == 7175 ? "CEFGA" : "GIGOV",
                    DtCriacao = x.DtCriacao,
                })
                .ToListAsync(cancellationToken);

            return mensagem;
        }
    }
}
