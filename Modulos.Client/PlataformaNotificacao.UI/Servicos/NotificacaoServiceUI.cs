using Plataforma.UI.Shared.Model;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace PlataformaNotificacao.UI.Servicos
{
    public class NotificacaoServiceUI
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpLocal;

        public NotificacaoServiceUI(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Api");
            _httpLocal = httpClientFactory.CreateClient("ApiLocal");
        }


        public async Task<int> CarregarContagemAsync(string matricula)
        {
            var response = await _httpClient.GetFromJsonAsync<int>($"api/notificacao/nao-lidas/total?usuarioId={matricula}");
            return response;
        }
        public async Task MarcarTodasLidas(string matricula)
        {
            await _httpClient.PutAsync($"api/notificacao/marcar-todas-lidas?usuarioId={matricula}", null);
        }
        public async Task MarcarLida(int codigoNotificacao,string matricula)
        {
            await _httpClient.PutAsync($"api/notificacao/{codigoNotificacao}/lida?usuarioId={matricula}", null);
        }
        public async Task<List<Notificacao>> CarregarHistoricoAsync(string matricula)
        {
            var lista = await _httpClient.GetFromJsonAsync<List<Notificacao>>($"api/notificacao/matricula?matricula={matricula}");
            return lista;
        }
    }
}
