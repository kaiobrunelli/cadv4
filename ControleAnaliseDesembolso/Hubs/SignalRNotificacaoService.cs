using Microsoft.AspNetCore.SignalR;
using PlataformaNotificacao.Domain;

namespace ControleAnaliseDesembolso.Hubs;

public class SignalRNotificacaoService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRNotificacaoService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task HandlerObserver(object? sender, MensagemNotificacao e)
    {
        Console.WriteLine(
            $"Escopo={e.Escopo} | Qtd={e.Destinatarios?.Count ?? 0} | Destinatarios={string.Join(",", e.Destinatarios ?? [])}");

        if (e.Destinatarios == null || e.Destinatarios.Count == 0)
        {
            Console.WriteLine("ERRO: notificação sem destinatários");
        }

        var destino = e.Destinatarios is { Count: > 0 }
            ? _hubContext.Clients.Groups(e.Destinatarios)
            : (IClientProxy)_hubContext.Clients.All;

        await destino.SendAsync(e.ChaveConexao, e);
    }
}
