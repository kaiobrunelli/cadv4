using Microsoft.JSInterop;
using PlataformaOperacional.Model.Encrip;
using System.Net.Http;
using System.Net.Http.Json;

public class EncryptionService
{
	private readonly IJSRuntime _js;
    private readonly HttpClient _httpClient;
    private readonly HttpClient _httpLocal;  
    public EncryptionService(IJSRuntime js, IHttpClientFactory httpClientFactory)
    {
		_js = js;
        _httpClient = httpClientFactory.CreateClient("Api");
        _httpLocal = httpClientFactory.CreateClient("ApiLocal");
    }
	public async Task<string> EncriptarAsync(string senha)
	{
		var chavePublica = await _httpClient.GetFromJsonAsync<ChavePublica>($"api/ConsultaChavePublica/");
		string resultado = await _js.InvokeAsync<string>(
					"criptografiaInterop.criptografarComChavePublica",
					senha,
					chavePublica.Key.Replace("\n", "").Replace("\r", "")
                );

		return resultado;
	}
}