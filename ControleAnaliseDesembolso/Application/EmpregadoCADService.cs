using ControleAnaliseDesembolso.Application.Interface;
using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using PlataformaNotificacao.Infra.Context;

namespace ControleAnaliseDesembolso.Application
{
    public class EmpregadoCADService : IEmpregadoCADService
    {
        private static readonly string[] _paletaCores =
        [
            "#005CA9", "#6D28D9", "#0E7490", "#065F46", "#92400E",
            "#7C3AED", "#B45309", "#BE185D", "#0F766E", "#1D4ED8",
        ];

        private readonly PlataformaNotificacaoContext _context;
        public EmpregadoCADService(PlataformaNotificacaoContext context) => _context = context;

        public async Task<List<Empregado>> ObterTodos() =>
            (await _context.EmpregadosAtivos
                .Where(e => e.CodigoSituacao != 9)
                .Select(e => new { e.Matricula, e.Nome })
                .ToListAsync())
            .Select(e => MapearEmpregado(e.Matricula!, e.Nome!))
            .ToList();

        public async Task<Empregado?> ObterPorMatricula(string matricula)
        {
            var empregado = await _context.EmpregadosAtivos
                .Where(e => e.Matricula == matricula)
                .Select(e => new { e.Matricula, e.Nome })
                .FirstOrDefaultAsync();

            return empregado is null ? null : MapearEmpregado(empregado.Matricula!, empregado.Nome!);
        }

        public async Task<List<Empregado>> ObterEmpregadosPorCoordenacao(string codigoCoordenacao) =>
            (await _context.EmpregadosAtivos
                .Where(e => e.Coordenacao == codigoCoordenacao && e.CodigoSituacao != 9)
                .Select(e => new { e.Matricula, e.Nome })
                .ToListAsync())
            .Select(e => MapearEmpregado(e.Matricula!, e.Nome!))
            .ToList();

        public async Task<List<string>> ObterCodigosGigovPorMatricula(string matricula) =>
            await _context.EmpregadosGigov
                .Where(e => e.Matricula == matricula && e.Ativo)
                .Select(e => e.CodigoGigov)
                .Distinct()
                .ToListAsync();

        private static Empregado MapearEmpregado(string matricula, string nome) => new()
        {
            Matricula = matricula,
            Nome = nome,
            Iniciais = ObterIniciais(nome),
            Cor = _paletaCores[Math.Abs(matricula.GetHashCode()) % _paletaCores.Length],
        };

        private static string ObterIniciais(string nome)
        {
            var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length switch
            {
                0 => "??",
                1 => partes[0].Length >= 2 ? partes[0][..2].ToUpperInvariant() : partes[0][..1].ToUpperInvariant(),
                _ => $"{partes[0][0]}{partes[^1][0]}".ToUpperInvariant(),
            };
        }
    }
}
