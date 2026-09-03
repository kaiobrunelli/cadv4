using RedeCaixaUtilitario.Application.Interface;
using RedeCaixaUtilitario.Domain.Model;

namespace RedeCaixaUtilitario.Application;

public class SiapfServiceMock : ISiapfService
{
    public async Task<CadastroGeralSiapf> ConsultarCadastroGeralAsync(
        string contrato,
        string contratoDv,
        string matricula,
        string senha,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(600, cancellationToken);

        if (contrato.Trim() == "0000000")
        {
            throw new InvalidOperationException("MOCK - Contrato não localizado no SIAPF (simulado).");
        }

        return new CadastroGeralSiapf
        {
            Contrato = contrato,
            ContratoDv = contratoDv,
            DadosGerais = new DadosGerais
            {
                DeMutuarioFinal = "PREFEITURA MUNICIPAL MOCK SIAPF",
                DeAgentePromotorOuParceiro = "AGENTE PROMOTOR MOCK SIAPF",
                DeObjetivo = "PRO-MORADIA - CONSTRUCAO DE UNIDADES HABITACIONAIS",
                CoMutuarioFinal = "12345",
                CoAgentePromotorOuParceiro = "67890",
                DtAssinatura = DateTime.Today.AddYears(-1),
            },
        };
    }
}
