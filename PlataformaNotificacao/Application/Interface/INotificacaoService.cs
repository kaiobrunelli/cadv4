using PlataformaNotificacao.Domain;
using PlataformaNotificacao.Domain.DTO;
using PlataformaNotificacao.Domain.Enum;

namespace PlataformaNotificacao.Application.Interface
{
    public interface INotificacaoService
    {
        event EventHandler<MensagemNotificacao>? OnNotificacao;

        Task EnviarGeralAsync(
      string titulo, string mensagem, TipoNotificacao tipo = TipoNotificacao.Normal,
      string? link = null, int? dias = null, int? horas = null,
      CancellationToken cancellationToken = default);

        Task EnviarModuloAsync(
            string titulo, string mensagem, TipoNotificacao tipo,
            CodigoAplicativo codigoAplicativo, string? link = null,
            int? dias = null, int? horas = null,
            CancellationToken cancellationToken = default);

        Task EnviarCoordenacaoAsync(
            string codigoCoordenacao,
            string titulo, string mensagem, TipoNotificacao tipo = TipoNotificacao.Normal,
            string? link = null, int? dias = null, int? horas = null,
            CancellationToken cancellationToken = default);

        Task EnviarPorMatriculasAsync(
             List<string> matriculas,
            string titulo, string mensagem, TipoNotificacao tipo = TipoNotificacao.Normal,
            CodigoAplicativo? codigoAplicativo = null, string? link = null,
            int? dias = null, int? horas = null,
            CancellationToken cancellationToken = default);

        Task EnviarIndividualAsync(
            string titulo, string mensagem,
           List<string> matriculas, CodigoAplicativo? codigoAplicativo = null,
            string? link = null, int? dias = null, int? horas = null,
            TipoNotificacao tipo = TipoNotificacao.Normal,
            CancellationToken cancellationToken = default);

        Task EnviarParaGigovAsync(
            string codigoGigov,
            string titulo, string mensagem, CodigoAplicativo? codigoAplicativo = null,
            string? link = null, int? dias = null, int? horas = null,
            TipoNotificacao tipo = TipoNotificacao.Normal,
            CancellationToken cancellationToken = default);

        Task EnviarParaResponsavelEGestorAsync(
            string? matriculaResponsavel, string codigoCoordenacao,
            string titulo, string mensagem, CodigoAplicativo? codigoAplicativo = null,
            string? link = null, int? dias = null, int? horas = null,
            TipoNotificacao tipo = TipoNotificacao.Normal,
            CancellationToken cancellationToken = default);

        Task<List<NotificacaoDto>> ObterNotificacaoPorMatriculaAsync(
            string codigoUsuario, int limite = 50, CancellationToken cancellationToken = default);

        Task<int> ContarNaoLidasAsync(string codigoUsuario, CancellationToken cancellationToken = default);

        Task<bool> MarcarLidaAsync(int codigoNotificacao, string codigoUsuario, CancellationToken cancellationToken = default);

        Task MarcarTodasLidasAsync(string codigoUsuario, CancellationToken cancellationToken = default);
    }
}
