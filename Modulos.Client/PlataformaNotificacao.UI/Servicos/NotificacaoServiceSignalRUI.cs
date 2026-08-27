using Plataforma.UI.Shared.Model;


namespace PlataformaNotificacao.UI.Servicos;

public class NotificacaoServiceSignalRUI
{
    private const string NomeEventoHub = "ReceberNotificacao";

    private readonly SignalRServiceUI _signalR;
    private bool _escutaRegistrada;

    public event Action<MensagemNotificacao>? AoReceberNotificacao;

    public bool Conectado => _signalR.Conectado;

    public string? UsuarioAtual => _signalR.UsuarioAtual;

    public NotificacaoServiceSignalRUI(SignalRServiceUI signalR)
    {
        _signalR = signalR;
    }

    public async Task IniciarOuReconectarAsync(string matriculaUsuario)
    {
        await _signalR.DefinirUsuarioAsync(matriculaUsuario);

        if (!_escutaRegistrada)
        {
            _escutaRegistrada = true;
            await _signalR.EscutarEvento<MensagemNotificacao>(NomeEventoHub, TratarMensagemAsync);
        }

        try
        {
            await _signalR.IniciarHubConnection();
        }
        catch
        {
        }
    }

    private Task TratarMensagemAsync(MensagemNotificacao msg)
    {
        Console.WriteLine($"[SignalR] Notificação recebida: {msg.Titulo} (escopo: {msg.Escopo})");

        if (msg.DataValidade != default && msg.DataValidade.ToLocalTime() < DateTime.Now)
            return Task.CompletedTask;

        AoReceberNotificacao?.Invoke(msg);
        return Task.CompletedTask;
    }

    public Task IniciarAsync(string matriculaUsuario)
    {
        return IniciarOuReconectarAsync(matriculaUsuario);
    }
}
