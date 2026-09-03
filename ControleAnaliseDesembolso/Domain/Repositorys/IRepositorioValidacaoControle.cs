using ControleAnaliseDesembolso.Domain.Entitys;

namespace ControleAnaliseDesembolso.Domain.Repositorys
{
    public interface IRepositorioValidacaoControle : IRepositorioBase<ValidacaoControleDesembolso>
    {
        Task<ValidacaoControleDesembolso?> BuscarValidacao(int CoValidacao, int CoControleDesembolso, CancellationToken cancellationToken);

    }
}
