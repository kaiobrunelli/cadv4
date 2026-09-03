using RedeCaixaUtilitario.Application.Interface;
using RedeCaixaUtilitario.Domain.Model;

namespace RedeCaixaUtilitario.Application;

// MOCK pra testar a tela de conferência (TB005/TB006) sem abrir sessão
// nenhuma no terminal 3270 — devolve um CadastroGeralSiapf fixo/fictício na
// hora. Implementa a MESMA ISiapfService que o SiapfService real; trocar um
// pelo outro é uma linha só no DI (ver Program.cs — "COMO VOLTAR AO NORMAL"
// no comentário lá).
//
// Este arquivo não é referenciado em nenhum outro lugar do código além do
// DI — pode apagar ele inteiro a qualquer momento que nada mais quebra.
public class SiapfServiceMock : ISiapfService
{
    public async Task<CadastroGeralSiapf> ConsultarCadastroGeralAsync(
        string contrato,
        string contratoDv,
        string matricula,
        string senha,
        CancellationToken cancellationToken = default)
    {
        // Delay pequeno só pra sentir a tela de "validando..." como se fosse
        // uma consulta de verdade. Pode tirar sem problema nenhum.
        await Task.Delay(600, cancellationToken);

        // Sentinela pra testar o caminho de falha/PENDENTE na tela: consultar
        // o contrato "0000000" simula erro de consulta ao SIAPF (mesma
        // mensagem que apareceria se o try/catch em
        // ExecutarConferenciaCamposInterno pegasse uma RedeCaixaException real).
        if (contrato.Trim() == "0000000")
        {
            throw new InvalidOperationException("MOCK - Contrato não localizado no SIAPF (simulado).");
        }

        return new CadastroGeralSiapf
        {
            // Ecoa o contrato pedido de volta -> item "SiapfContratoAf" some
            // sempre como OK (é a mesma lógica que o real: a tela ecoa o
            // contrato consultado).
            Contrato = contrato,
            ContratoDv = contratoDv,
            DadosGerais = new DadosGerais
            {
                // Valores fixos de propósito — como não têm relação com o
                // desembolso real que você estiver testando, tendem a
                // aparecer como ERRO em "SiapfMutuarioFinal"/
                // "SiapfAgentePromotor"/"SiapfPrograma", o que é bom pra ver
                // a tela mostrando os dois estados (OK e ERRO) ao mesmo
                // tempo. Se quiser ver um desembolso específico 100% OK,
                // troque esses três valores pelos mesmos do registro que
                // você estiver validando (MutuarioFinal, AgentePromotor e o
                // texto de exibição do Programa).
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
