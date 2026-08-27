using Microsoft.EntityFrameworkCore;
using PlataformaNotificacao.Application.Interface;
using PlataformaNotificacao.Domain;
using PlataformaNotificacao.Domain.DTO;
using PlataformaNotificacao.Domain.Enum;
using PlataformaNotificacao.Infra.Context;

namespace PlataformaNotificacao.Application
{
    public class NotificacaoService : INotificacaoService
    {

        public event EventHandler<MensagemNotificacao>? OnNotificacao;
        public EmpregadoService empregados;
        private readonly PlataformaNotificacaoContext db;

        public NotificacaoService(string chave)
        {
            db = new PlataformaNotificacaoContext(chave);
            empregados = new EmpregadoService(db);
        }

        public async Task EnviarGeralX(string matricula)
        {
            var matriculas = empregados.ObterMatriculasTodos();

            var notificacao = CriarNotifX();
            notificacao.Destinatarios = matriculas.Select(m => new ControleVisualizacao
            {
                CodigoUsuario = m,
                Link = "123"
            }).ToList();

        }
        private Notificacao CriarNotifX() => new Notificacao();
       
        public async Task EnviarGeralAsync(
       string titulo, string mensagem, TipoNotificacao tipo = TipoNotificacao.Normal,
       string? link = null, int? dias = null, int? horas = null,
       CancellationToken cancellationToken = default)
        {
            var matriculas = await empregados.ObterTodasMatriculas();
            var notif = CriarNotificacao(titulo, mensagem, tipo, null, dias, horas);

            notif.Destinatarios = matriculas
                .Select(mat => new ControleVisualizacao { CodigoUsuario = mat, Link = link })
                .ToList();

            db.Notificacoes.Add(notif);
            await db.SaveChangesAsync(cancellationToken);

            OnNotificacao?.Invoke(this, ToMensagem(notif, link, EscopoNotificacao.Geral, matriculas));
        }

        public async Task EnviarModuloAsync(
            string titulo, string mensagem, TipoNotificacao tipo,
            CodigoAplicativo codigoAplicativo, string? link = null,
            int? dias = null, int? horas = null,
            CancellationToken cancellationToken = default)
        {
            var matriculas = empregados.ObterMatriculasPorModulo(codigoAplicativo.ToString());
            var notif = CriarNotificacao(titulo, mensagem, tipo, codigoAplicativo, dias, horas);

            notif.Destinatarios = matriculas
                .Select(mat => new ControleVisualizacao { CodigoUsuario = mat, Link = link })
                .ToList();

            db.Notificacoes.Add(notif);
            await db.SaveChangesAsync(cancellationToken);

            OnNotificacao?.Invoke(this, ToMensagem(notif, link, EscopoNotificacao.Modulo, matriculas));
        }

        public async Task EnviarCoordenacaoAsync(
            string codigoCoordenacao,
            string titulo, string mensagem, TipoNotificacao tipo = TipoNotificacao.Normal,
            string? link = null, int? dias = null, int? horas = null,
            CancellationToken cancellationToken = default)
        {
            var matriculas = await empregados.ObterMatriculasPorCoordenacao(codigoCoordenacao);
            await EnviarIndividualAsync(titulo, mensagem, matriculas, null, link, dias, horas, tipo, cancellationToken);
        }

        public async Task EnviarPorMatriculasAsync(
            List<string> matriculas,
            string titulo, string mensagem, TipoNotificacao tipo = TipoNotificacao.Normal,
            CodigoAplicativo? codigoAplicativo = null, string? link = null,
            int? dias = null, int? horas = null,
            CancellationToken cancellationToken = default)
        {
            await EnviarIndividualAsync(titulo, mensagem, matriculas, codigoAplicativo, link, dias, horas, tipo, cancellationToken);
        }

        public async Task EnviarIndividualAsync(
            string titulo, string mensagem,
              List<string> matriculas, CodigoAplicativo? codigoAplicativo = null,
            string? link = null, int? dias = null, int? horas = null,
            TipoNotificacao tipo = TipoNotificacao.Normal,
            CancellationToken cancellationToken = default)
        {
            var notif = CriarNotificacao(titulo, mensagem, tipo, codigoAplicativo, dias, horas);

            notif.Destinatarios = matriculas
                .Select(mat => new ControleVisualizacao { CodigoUsuario = mat, Link = link })
                .ToList();

            db.Notificacoes.Add(notif);
            await db.SaveChangesAsync(cancellationToken);

            OnNotificacao?.Invoke(this, ToMensagem(notif, link, EscopoNotificacao.Individual, matriculas));
        }

        public async Task EnviarParaGigovAsync(
            string codigoGigov,
            string titulo, string mensagem, CodigoAplicativo? codigoAplicativo = null,
            string? link = null, int? dias = null, int? horas = null,
            TipoNotificacao tipo = TipoNotificacao.Normal,
            CancellationToken cancellationToken = default)
        {
            var matriculas = await empregados.ObterMatriculasGigovPorNumero(codigoGigov);
            if (matriculas.Count == 0) return;

            await EnviarIndividualAsync(titulo, mensagem, matriculas, codigoAplicativo, link, dias, horas, tipo, cancellationToken);
        }

        public async Task EnviarParaResponsavelEGestorAsync(
            string? matriculaResponsavel, string codigoCoordenacao,
            string titulo, string mensagem, CodigoAplicativo? codigoAplicativo = null,
            string? link = null, int? dias = null, int? horas = null,
            TipoNotificacao tipo = TipoNotificacao.Normal,
            CancellationToken cancellationToken = default)
        {
            var destinatarios = new List<string>();
            if (!string.IsNullOrWhiteSpace(matriculaResponsavel))
                destinatarios.Add(matriculaResponsavel);

            var gestor = await empregados.ObterGestorAtivoOuEventualAsync(codigoCoordenacao);
            if (gestor is not null && !destinatarios.Contains(gestor))
                destinatarios.Add(gestor);

            if (destinatarios.Count == 0) return;

            await EnviarIndividualAsync(titulo, mensagem, destinatarios, codigoAplicativo, link, dias, horas, tipo, cancellationToken);
        }

        public async Task<List<NotificacaoDto>> ObterNotificacaoPorMatriculaAsync(
            string codigoUsuario, int limite = 50, CancellationToken cancellationToken = default)
        {
            var agora = DateTime.UtcNow;
            return await db.ControleVisualizacoes
                .Where(cv => cv.CodigoUsuario == codigoUsuario && cv.Notificacao.DataValidade > agora)
                .OrderByDescending(cv => cv.Notificacao.DataCriacao)
                .Take(limite)
                .Select(cv => new NotificacaoDto
                {
                    CodigoNotificacao = cv.Notificacao.CodigoNotificacao,
                    Titulo = cv.Notificacao.Titulo,
                    Mensagem = cv.Notificacao.Mensagem,
                    Tipo = cv.Notificacao.Tipo,
                    CodigoAplicativo = cv.Notificacao.CodigoAplicativo,
                    DataCriacao = cv.Notificacao.DataCriacao,
                    DataValidade = cv.Notificacao.DataValidade,
                    DataVisualizacao = cv.DataVisualizacao,
                    Link = cv.Link
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> ContarNaoLidasAsync(string codigoUsuario, CancellationToken cancellationToken = default)
        {
            var agora = DateTime.UtcNow;
            return await db.ControleVisualizacoes
                .Where(cv => cv.CodigoUsuario == codigoUsuario
                          && cv.DataVisualizacao == null
                          && cv.Notificacao.DataValidade > agora)
                .CountAsync(cancellationToken);
        }

        public async Task<bool> MarcarLidaAsync(int codigoNotificacao, string codigoUsuario, CancellationToken cancellationToken = default)
        {
            var cv = await db.ControleVisualizacoes
                .FirstOrDefaultAsync(x => x.CodigoNotificacao == codigoNotificacao
                                       && x.CodigoUsuario == codigoUsuario, cancellationToken);

            if (cv is null || cv.DataVisualizacao != null) return false;

            cv.DataVisualizacao = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task MarcarTodasLidasAsync(string codigoUsuario, CancellationToken cancellationToken = default)
        {
            var agora = DateTime.UtcNow;
            var naoLidas = await db.ControleVisualizacoes
                .Where(cv => cv.CodigoUsuario == codigoUsuario
                          && cv.DataVisualizacao == null
                          && cv.Notificacao.DataValidade > agora)
                .ToListAsync(cancellationToken);

            foreach (var cv in naoLidas)
                cv.DataVisualizacao = agora;

            await db.SaveChangesAsync(cancellationToken);
        }


        private static Notificacao CriarNotificacao(
            string titulo, string mensagem, TipoNotificacao tipo,
            CodigoAplicativo? codigoAplicativo,
            int? dias, int? horas) => new()
            {
                Titulo = titulo,
                Mensagem = mensagem,
                Tipo = tipo,
                CodigoAplicativo = codigoAplicativo,
                DataValidade = CalcularExpiracao(dias, horas)
            };

        private static DateTime CalcularExpiracao(int? dias, int? horas)
        {
            if (dias is null && horas is null) return DateTime.UtcNow.AddDays(7);
            return DateTime.UtcNow.AddDays(dias ?? 0).AddHours(horas ?? 0);
        }

        private static MensagemNotificacao ToMensagem(
            Notificacao n, string? link, EscopoNotificacao escopo, List<string> matriculas) => new()
        {
            CodigoNotificacao = n.CodigoNotificacao,
            Titulo = n.Titulo,
            Mensagem = n.Mensagem,
            Tipo = n.Tipo,
            Escopo = escopo,
            CodigoAplicativo = n.CodigoAplicativo,
            Link = link,
            CriadaEm = n.DataCriacao,
            DataValidade = n.DataValidade,
            Destinatarios = matriculas
        };
    }
}
