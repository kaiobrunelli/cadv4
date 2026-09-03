using RedeCaixaUtilitario.Domain.Model;

namespace RedeCaixaUtilitario.Application.Interface;

public interface ISiapfService
{
    Task<CadastroGeralSiapf> ConsultarCadastroGeralAsync(
        string contrato,
        string contratoDv,
        string matricula,
        string senha,
        CancellationToken cancellationToken = default);
}
