using RedeCaixaUtilitario.Application.Interface;
using RedeCaixaUtilitario.Domain.Model;

namespace RedeCaixaUtilitario.Application;

// Versão simulada do RedeCaixaUtilitario real (XP Metodo nvoo/RedeCaixaUtilitario) — só o
// suficiente pra simular a injeção do SiapfService dentro do CAD. O ISiapfService real tem
// muito mais métodos (DRP, movimentação financeira, cronograma, acesso ao CER...), todos
// dependentes de terminal 3270 (Helpers.RedeCaixa/IRedeCaixa). Aqui ficou só a região
// "Cadastro" (ConsultarCadastroGeral), que é o método usado pra confrontar valores na
// validação do FPD.
public class SiapfService : ISiapfService
{
    public Task<CadastroGeralSiapf> ConsultarCadastroGeralAsync(string contrato, string contratoDv, CancellationToken cancellationToken = default)
    {
        // SIMULADO — no projeto real, este método navegava pela tela MB010 do SIAPF (ver
        // RedeCaixaUtilitario.Application.SiapfService.ConsultarCadastroGeral, região
        // "Cadastro") e montava o CadastroGeralSiapf lendo campo a campo da tela do
        // terminal. Aqui devolvemos valores fixos só pra ter algo real pra confrontar
        // contra o FPD. Ajuste os valores abaixo pra forçar aprovação/divergência nos
        // testes manuais da validação de VALORES.
        var cadastro = new CadastroGeralSiapf
        {
            Contrato = contrato,
            ContratoDv = contratoDv,
            DadosGerais = new DadosGeraisSiapf
            {
                Tomador = "53000",
                MutuarioFinal = "Prefeitura Simulada",
                DtAssinatura = new DateTime(2024, 1, 1),
            },
            DadosFinanceiros = new DadosFinanceirosSiapf
            {
                ValorEmprestimo = 250000m,
                ValorContrapartida = 50000m,
                SaldoIntegralizado = 32000m,
                SaldoDevedor = 218000m,
            },
        };

        return Task.FromResult(cadastro);
    }
}
