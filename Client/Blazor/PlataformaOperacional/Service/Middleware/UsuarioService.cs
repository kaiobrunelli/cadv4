using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using PlataformaOperacional.Model.CentralPermissoes;
using PlataformaOperacional.Model.Contabilidade;
using PlataformaOperacional.Model.Plataforma;
using PlataformaOperacional.Model.Usuario;
using System.Net.Http.Json;
using System.Security.Claims;

namespace PlataformaOperacional.Service.Middleware
{
	public class UsuarioService
	{

		private readonly HttpClient _httpClient;
		private readonly HttpClient _httpLocal;
		private readonly BlazorMockService _mockBlazor;
		private readonly ISnackbar _snackbar;

		public UsuarioService(IHttpClientFactory httpClientFactory, BlazorMockService blazorMockService, ISnackbar snackbar)
		{
			_httpClient = httpClientFactory.CreateClient("Api");
			_httpLocal = httpClientFactory.CreateClient("ApiLocal");
			_mockBlazor = blazorMockService;
			_snackbar = snackbar;
		}




		public async Task<ConfiguracoesUsuario> ConsultaConfiguracaoUsuario()
		{
			if (_mockBlazor.MockarDados)
			{
				return await _httpLocal.GetFromJsonAsync<ConfiguracoesUsuario>("sample-data/ConfiguracaoUsuario.json")
					?? new ConfiguracoesUsuario();
			}
			else
			{
				try
				{
					var response = await _httpClient.GetAsync("api/ConsultaConfiguracoesUsuario");
					if (response.IsSuccessStatusCode)
					{
						return await response.Content.ReadFromJsonAsync<ConfiguracoesUsuario>();
					}
				}
				catch (Exception)
				{
					// Servidor de configurações (PlataformaOperacionalAPI) inalcançável
					// nesse ambiente — cai pro fallback abaixo igual a uma resposta
					// não-sucesso.
				}

				_snackbar.Add("Falha ao consultar permissões de usuário. Usando identidade local de teste.", Severity.Warning);
				return await _httpLocal.GetFromJsonAsync<ConfiguracoesUsuario>("sample-data/ConfiguracaoUsuario.json")
					?? new ConfiguracoesUsuario();
			}


		}

		private async Task<ConfiguracoesUsuario> GetStaticFallbackData()
		{
			return await _httpClient.GetFromJsonAsync<ConfiguracoesUsuario>(
					  "https://localhost:7224/sample-data/ConfiguracaoUsuario.json"
			) ?? new ConfiguracoesUsuario();
		}

		public async Task<Usuario> ConsultaUsuario()
		{

			var response = await _httpClient.GetFromJsonAsync<Usuario>("api/ConsultaUsuario");
			if (response == null)
			{
				throw new Exception($"Usuário não reconhecido.");

			}
			return response;
		}

	}
}
