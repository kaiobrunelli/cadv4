using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MudBlazor;
using PlataformaOperacional.Model.AplicacaoModel;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;

namespace PlataformaOperacional.Service.AplicacaoService
{
    public class SaldoResidualService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpLocal;

        public SaldoResidualService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Api");
            _httpLocal = httpClientFactory.CreateClient("ApiLocal");
        }
        public async Task<HttpResponseMessage> LocalizarContratosParaAnalise()
        {
			var response = await _httpClient.GetAsync("api/LocalizarContratosParaAnalise");
			return response;
		
        }
        public async Task<List<SaldoResidualContrato>> LocalizarContratosNaoProcessados()
        {
            var response = await _httpClient.GetFromJsonAsync<List<SaldoResidualContrato>>("api/LocalizarContratosNaoProcessados");
            if (response == null)
            {
                throw new Exception($"Não foi encontrado Contrato não processado.");
            }
            return response;
        }
			


		public async Task<HttpResponseMessage> LocalizarSituacaoOperacoes()
        {
			var response = await _httpClient.GetAsync("api/LocalizarSituacaoOperacoes");
            return response;
            
        }
        public async Task<HttpResponseMessage> TaskRealizarOperacaoSaldoResidual(string senha)
        {   
            var response = await _httpClient.PostAsJsonAsync($"api/RealizarOperacaoSaldoResidual", senha);         
            return response;
		}
        public async Task<HttpResponseMessage> ProcessarContratoManual(ProcessarContratoManual contratoManual)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/ProcessarContratoManual", contratoManual);
            return response;
        }
    }
}


