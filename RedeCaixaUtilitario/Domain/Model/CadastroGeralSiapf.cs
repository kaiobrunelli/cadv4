namespace RedeCaixaUtilitario.Domain.Model;

// Versão simplificada de RedeCaixaUtilitario.Domain.Model.CadastroGeralSiapf (projeto real em
// "XP Metodo nvoo/RedeCaixaUtilitario"). O original tem ~9 classes aninhadas (DadosGerais,
// DadosObra, DadosCadastraisAcompanhamento, DadosCadastraisCarencia, DadosCadastraisRetorno,
// DadosComplementares...) cobrindo a tela inteira de cadastro do contrato (região "Cadastro" /
// método ConsultarCadastroGeral do SiapfService real). Aqui ficou só o suficiente pra manter o
// formato "uma classe com várias classes dentro" e ter campos financeiros pra confrontar com o FPD.
public class CadastroGeralSiapf
{
    public string Contrato { get; set; } = string.Empty;
    public string ContratoDv { get; set; } = string.Empty;
    public DadosGeraisSiapf DadosGerais { get; set; } = new();
    public DadosFinanceirosSiapf DadosFinanceiros { get; set; } = new();
}

public class DadosGeraisSiapf
{
    public string? Tomador { get; set; }
    public string? MutuarioFinal { get; set; }
    public DateTime DtAssinatura { get; set; }
}

public class DadosFinanceirosSiapf
{
    public decimal ValorEmprestimo { get; set; }
    public decimal ValorContrapartida { get; set; }
    public decimal SaldoIntegralizado { get; set; }
    public decimal SaldoDevedor { get; set; }
}
