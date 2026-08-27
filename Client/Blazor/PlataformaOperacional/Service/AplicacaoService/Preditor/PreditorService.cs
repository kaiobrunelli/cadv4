
using PlataformaOperacional.Model.Aplicacao.Preditor;
using PlataformaOperacional.Model.Plataforma;
using PlataformaOperacional.Service.Middleware;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace PlataformaOperacional.Service.AplicacaoService.Preditor
{
	public class PreditorService
	{
	
		private readonly HttpClient _httpClient;
		private readonly HttpClient _httpLocal;
		private readonly BlazorMockService _mockBlazor;

		public PreditorService(IHttpClientFactory httpClientFactory, BlazorMockService blazorMockService)
		{
			_httpClient = httpClientFactory.CreateClient("Api");

			_httpLocal = httpClientFactory.CreateClient("ApiLocal");				
			_mockBlazor = blazorMockService;
		}

	
		public async Task<HttpResponseMessage> ConsultarDadosDoPreditorDeDesembolso()
		{
			
			if (_mockBlazor.MockarDados)
			{
				var repsonseMock = await _httpLocal.GetAsync("sample-data/Preditor/ControlePreditorDeDesembolso.json");
				return repsonseMock;
			}
			var response = await _httpClient.GetAsync("api/ConsultarDadosDoPreditorDeDesembolso");
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarPreditorAnaliticoPublico()
		{
			var response = await _httpClient.GetAsync("api/ConsultarPreditorAnaliticoPublico");
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarPreditorAnaliticoPrivado()
		{
			var response = await _httpClient.GetAsync("api/ConsultarPreditorAnaliticoPrivado");
			return response;
		}
        public async Task<HttpResponseMessage> ConsultarRelatorioDeResultados()
        {
            var response = await _httpClient.GetAsync("api/ConsultarRelatorioDeResultados");
            return response;
        }
		
		public async Task<HttpResponseMessage> AtualizarPreditorAnaliticoPrivado(AnaliticoSetorPrivado? valor)
		{
			var response = await _httpClient.PostAsJsonAsync("api/AtualizarPreditorAnaliticoPrivado", valor);
			return response;
		}
		public async Task<HttpResponseMessage> AtualizarPreditorAnaliticoPublico(AnaliticoSetorPublico? valor)
		{
			var response = await _httpClient.PostAsJsonAsync("api/AtualizarPreditorAnaliticoPublico", valor);
			return response;
		}
		public async Task<HttpResponseMessage> ResetarPreditorAnaliticoPrivado()
		{
			var response = await _httpClient.PostAsync("api/ResetarPreditorAnaliticoPrivado", null);
			return response;
		}
		public async Task<HttpResponseMessage> ResetarPreditorAnaliticoPublico()
		{
			var response = await _httpClient.PostAsync("api/ResetarPreditorAnaliticoPublico", null);
			return response;
		}
		public async Task<HttpResponseMessage> FinalizarPreditorEnviarEmail()
		{
			var response = await _httpClient.PostAsync("api/FinalizarPreditorEnviarEmail", null);
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarPreditorAutorizados()
		{
			HttpResponseMessage response;
			if (_mockBlazor.MockarDados)
			{
				response = await _httpLocal.GetAsync("sample-data/Preditor/PreditorAutorizados.json");
				return response;
			}

			response = await _httpClient.GetAsync("api/ConsultarPreditorAutorizados");
			return response;
		}

		public async Task<HttpResponseMessage> CadastrarPreditorAutorizado(CreatePreditorAutorizados cadastro)
		{
			var response = await _httpClient.PostAsJsonAsync("api/CadastrarPreditorAutorizado", cadastro);
			return response;
		}
		public async Task<HttpResponseMessage> DesabilitarPreditorAutorizado(string matricula)
		{
			var response = await _httpClient.PostAsJsonAsync("api/DesabilitarPreditorAutorizado", matricula);
			return response;
		}

		public async Task<HttpResponseMessage> CadastrarEventual(string matricula)
		{
			var response = await _httpClient.PostAsJsonAsync("api/CadastrarEventual", matricula);
			return response;
		}
		public async Task<HttpResponseMessage> RemoverEventual(string matricula)
		{
			var response = await _httpClient.PostAsJsonAsync("api/RemoverEventual", matricula);
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarEmailPreditor()
		{
			HttpResponseMessage response;
			if (_mockBlazor.MockarDados)
			{
				response = await _httpLocal.GetAsync("sample-data/Preditor/EmailEmail.json");
				return response;
			}

			response = await _httpClient.GetAsync("api/ConsultarEmailPreditor");
			return response;
		}
		public async Task<HttpResponseMessage> CadastrarEmailPreditor(CreateDestinatarioEmailPreditor destinatarioEmail)
		{
			var response = await _httpClient.PostAsJsonAsync("api/CadastrarEmailPreditor", destinatarioEmail);
			return response;
		}

		public async Task<HttpResponseMessage> RemoverEmailPreditor(int coDestinatario)
		{
			var response = await _httpClient.PostAsJsonAsync("api/RemoverEmailPreditor", coDestinatario);
			return response;
		}












	}
}

