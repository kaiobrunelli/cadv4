using Microsoft.EntityFrameworkCore;
using PlataformaNotificacao.Application.Interface;
using PlataformaNotificacao.Domain.Enum;
using PlataformaNotificacao.Infra.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaNotificacao.Application
{
    public class EmpregadoService : IEmpregadoService
    {
        private readonly PlataformaNotificacaoContext _context;
        public EmpregadoService(PlataformaNotificacaoContext context) => _context = context;

        private static readonly List<Empregado> _todos =
        [
        new() { Matricula = "c123456", Nome = "Ana Lima",         Iniciais = "AL", Cargo = "Analista Sênior",     Cor = "#005CA9", Modulos = ["Sipub", "Cobranca"],                                  CodigoCoordenacao = "GIGOV01" },
        new() { Matricula = "c102944", Nome = "Bruno Costa",      Iniciais = "BC", Cargo = "Gestor",              Cor = "#065F46", Modulos = ["Sipub", "EncontroDeContas"],                         CodigoCoordenacao = "06" },
        new() { Matricula = "c134872", Nome = "Carla Mendes",     Iniciais = "CM", Cargo = "Analista Júnior",     Cor = "#7C3AED", Modulos = ["Sipub"],                                             CodigoCoordenacao = "GIGOV02" },
        new() { Matricula = "c110233", Nome = "Diego Santos",     Iniciais = "DS", Cargo = "Coordenador",         Cor = "#B45309", Modulos = ["Cobranca", "Amortizacao"],                           CodigoCoordenacao = "06" },
        new() { Matricula = "c145097", Nome = "Elena Ferreira",   Iniciais = "EF", Cargo = "Diretora Financeira", Cor = "#BE185D", Modulos = ["Sipub", "Cobranca", "Amortizacao", "EncontroDeContas"], CodigoCoordenacao = "06" },
        new() { Matricula = "c151896", Nome = "Kaio KBS",   Iniciais = "KB", Cargo = "Programador", Cor = "#BE185D", Modulos = ["Sipub", "Cobranca", "Amortizacao", "EncontroDeContas"], CodigoCoordenacao = "06" },
    ];

        public List<Empregado> ObterTodos() => _todos;

        public Empregado? ObterPorMatricula(string matricula) =>
            _todos.FirstOrDefault(e => e.Matricula == matricula);

        public List<string> ObterMatriculasTodos() =>
            _todos.Select(e => e.Matricula).ToList();

        public List<string> ObterMatriculasPorModulo(string modulo) =>
            _todos.Where(e => e.Modulos.Contains(modulo)).Select(e => e.Matricula).ToList();

        public List<string> FiltrarMatriculasValidas(IEnumerable<string> matriculas) =>
            _todos.Where(e => matriculas.Contains(e.Matricula)).Select(e => e.Matricula).ToList();

        public async Task<List<string>> ObterTodasMatriculas() =>
            await _context.EmpregadosAtivos.Select(m => m.Matricula!).ToListAsync();

        public async Task<List<string>> ObterMatriculasPorCoordenacao(string codigoCoordenacao) =>
            await _context.EmpregadosAtivos
                .Where(e => e.Coordenacao == codigoCoordenacao && e.CodigoSituacao != 9)
                .Select(e => e.Matricula!)
                .ToListAsync();

        private const int FuncaoCoordenador = 1;
        private const int SituacaoAtivo = 1;
        private const int SituacaoFerias = 2;

        public async Task<List<string>> ObterMatriculasGigov() =>
            await _context.EmpregadosGigov
                .Where(e => e.Ativo)
                .Select(e => e.Matricula)
                .ToListAsync();

        public async Task<List<string>> ObterMatriculasGigovPorNumero(string codigoGigov) =>
            await _context.EmpregadosGigov
                .Where(e => e.Ativo && e.CodigoGigov == codigoGigov)
                .Select(e => e.Matricula)
                .ToListAsync();

        public async Task<string?> ObterGestorAtivoOuEventualAsync(string codigoCoordenacao)
        {
            var coordenador = await _context.EmpregadosAtivos
                .FirstOrDefaultAsync(e => e.Coordenacao == codigoCoordenacao && e.CodigoFuncao == FuncaoCoordenador);

            if (coordenador is null) return null;

            if (coordenador.CodigoSituacao == SituacaoAtivo)
                return coordenador.Matricula;

            if (coordenador.CodigoSituacao == SituacaoFerias && coordenador.CodigoEventual is not null)
            {
                var eventual = await _context.EmpregadosAtivos
                    .FirstOrDefaultAsync(e => e.CodigoEmpregado == coordenador.CodigoEventual);
                return eventual?.Matricula;
            }

            return null;
        }
    }
}
public class Empregado
{
    public string Matricula { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Iniciais { get; set; } = "";
    public string Cargo { get; set; } = "";
    public string Cor { get; set; } = "#005CA9";
    public string[] Modulos { get; set; } = [];
    public string CodigoCoordenacao { get; set; } = "";
}