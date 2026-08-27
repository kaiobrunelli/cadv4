using Plataforma.UI.Shared.Model;
using PlataformaOperacional.Model.Plataforma;

namespace PlataformaOperacional.Service.Middleware
{
    public class ServicoNotificacao
    {
        private const string NomeEventoHub = "ReceberNotificacao";

        private readonly SignalRService _signalR;
        private bool _escutaRegistrada;

        public event Action<MensagemNotificacao>? AoReceberNotificacao;

        public bool Conectado => _signalR.Conectado;

        public string? UsuarioAtual => _signalR.UsuarioAtual;

        public ServicoNotificacao(SignalRService signalR)
        {
            _signalR = signalR;
        }

        public async Task IniciarOuReconectarAsync(string usuarioId)
        {
            await _signalR.DefinirUsuarioAsync(usuarioId);

            if (!_escutaRegistrada)
            {
                _escutaRegistrada = true;
                await _signalR.EscutarEvento<MensagemNotificacao>(NomeEventoHub, TratarMensagemAsync);
            }

            await _signalR.IniciarHubConnection();
        }

        private Task TratarMensagemAsync(MensagemNotificacao msg)
        {
            Console.WriteLine($"[SignalR] Notificação recebida: {msg.Titulo} (escopo: {msg.Escopo})");

            if (msg.DataValidade != default && msg.DataValidade.ToLocalTime() < DateTime.Now)
                return Task.CompletedTask;

            AoReceberNotificacao?.Invoke(msg);
            return Task.CompletedTask;
        }


        public Task IniciarAsync(string usuarioId)
        {
            return IniciarOuReconectarAsync(usuarioId);
        }
    }

}