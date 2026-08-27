using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Infra.Datas.Context;

namespace ControleAnaliseDesembolso.Infra.Datas.Repositorys
{
    public class RepositorioFichaPedidoDesembolso : RepositorioBase<Desembolso>, IRepositorioFichaPedidoDesembolso
    {
        public RepositorioFichaPedidoDesembolso(ControleAnaliseDesembolsoContext context) : base(context)
        {
        }
    }
}
