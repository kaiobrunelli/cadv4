using RedeCaixaUtilitario.Application.Interface;
using RedeCaixaUtilitario.Domain.Model;

namespace RedeCaixaUtilitario.Application;

// Implementação PROVISÓRIA de IPlataformaOperacionalService — só existe pra
// manter a aplicação de pé (DI precisa de alguma implementação registrada)
// enquanto a plataforma não entrega a versão real (que vai identificar o
// usuário a partir do contexto de autenticação de verdade, não de um valor
// fixo). Troque essa classe pela implementação real assim que ela existir —
// não adicione lógica de negócio aqui.
public class PlataformaOperacionalServiceTemporario : IPlataformaOperacionalService
{
    // MATRICULA CHAPADA DE PROPÓSITO — troque aqui (e só aqui, é a única
    // ocorrência) quando a identificação real do usuário autenticado entrar.
    // "c151896" é a mesma matrícula de referência (CEFGA) já usada em outros
    // pontos do projeto (seed, PaginaAnalise.razor).
    private const string MatriculaFixaTemporaria = "c151896";

    public Usuario IdentificarUsuario() => new() { Matricula = MatriculaFixaTemporaria, Senha = string.Empty };

    public PedidoDeAutomacao GerarPedidoDeAutomacao(string area, string endpoint, string senha, bool assincrono)
    {
        var usuario = IdentificarUsuario();
        usuario.Senha = senha;

        return new PedidoDeAutomacao
        {
            Area = area,
            Servico = endpoint,
            MatriculaSolicitante = usuario.Matricula,
            DtSolicitacao = DateTime.Now,
            Usuario = usuario,
        };
    }

    public TrilhaDeAuditoria GerarTrilhaDeAuditoria(string evento, string descEvento, string respostaInicial, string endpointDisplayName) => new()
    {
        Evento = evento,
        DescEvento = descEvento,
        Resposta = respostaInicial,
        EndpointDisplayName = endpointDisplayName,
        DtSolicitacao = DateTime.Now,
    };

    public Task SaveAsync() => Task.CompletedTask;
}
