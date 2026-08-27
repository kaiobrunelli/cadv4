using PlataformaNotificacao.Application.Interface;
using PlataformaNotificacao.Domain;

namespace PlataformaNotificacao.Application;

public class EmpregadoService : IEmpregadoService
{
    private static readonly List<Empregado> _todos =
    [
        new() { Matricula = "c123456", Nome = "Ana Lima",         Iniciais = "AL", Cargo = "Analista Sênior",     Cor = "#005CA9", Modulos = ["Sipub", "Cobranca"],                                  CodigoCoordenacao = "COORD-SIPUB" },
        new() { Matricula = "c102944", Nome = "Bruno Costa",      Iniciais = "BC", Cargo = "Gestor",              Cor = "#065F46", Modulos = ["Sipub", "EncontroDeContas"],                         CodigoCoordenacao = "COORD-SIPUB" },
        new() { Matricula = "c134872", Nome = "Carla Mendes",     Iniciais = "CM", Cargo = "Analista Júnior",     Cor = "#7C3AED", Modulos = ["Sipub"],                                             CodigoCoordenacao = "COORD-SIPUB" },
        new() { Matricula = "c110233", Nome = "Diego Santos",     Iniciais = "DS", Cargo = "Coordenador",         Cor = "#B45309", Modulos = ["Cobranca", "Amortizacao"],                           CodigoCoordenacao = "COORD-FIN"   },
        new() { Matricula = "c145097", Nome = "Elena Ferreira",   Iniciais = "EF", Cargo = "Diretora Financeira", Cor = "#BE185D", Modulos = ["Sipub", "Cobranca", "Amortizacao", "EncontroDeContas"], CodigoCoordenacao = "COORD-DIR"  },
        new() { Matricula = "c151896", Nome = "Kaio KBS",         Iniciais = "KB", Cargo = "Programador",         Cor = "#BE185D", Modulos = ["Sipub", "Cobranca", "Amortizacao", "EncontroDeContas"], CodigoCoordenacao = "COORD-DIR"  },
    ];

    public List<Empregado> ObterTodos() => _todos;

    public Empregado? ObterPorMatricula(string matricula) =>
        _todos.FirstOrDefault(e => e.Matricula == matricula);

    public List<string> ObterMatriculasTodos() =>
        _todos.Select(e => e.Matricula).ToList();

    public List<string> ObterMatriculasPorModulo(string modulo) =>
        _todos.Where(e => e.Modulos.Contains(modulo)).Select(e => e.Matricula).ToList();

    public List<string> ObterMatriculasPorCoordenacao(string codigoCoordenacao) =>
        _todos.Where(e => e.CodigoCoordenacao == codigoCoordenacao).Select(e => e.Matricula).ToList();

}
