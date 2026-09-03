using RedeCaixaUtilitario.Domain.Model;

namespace RedeCaixaUtilitario.Application.Interface;

public interface ISiapfService
{
    // matricula/senha são as credenciais de quem clicou em "Validar" no CAD —
    // usadas só pra logar no SIAPF nessa consulta, não ficam guardadas.
    Task<CadastroGeralSiapf> ConsultarCadastroGeralAsync(
        string contrato,
        string contratoDv,
        string matricula,
        string senha,
        CancellationToken cancellationToken = default);
}
