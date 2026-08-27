using PlataformaOperacional.Model.CentralPermissoes;
using PlataformaOperacional.Model.Cobranca;
using PlataformaOperacional.Model.Cobranca.Amortizacao;
using PlataformaOperacional.Service.Middleware;
using System.Net.Http;
using System.Net.Http.Json;

namespace PlataformaOperacional.Service.Cobranca
{
	public class CobrancaService
	{
		private readonly HttpClient _httpClient;
		private readonly HttpClient _httpLocal;
		private readonly BlazorMockService _mockBlazor;
		private DownloadService _downloadService;
		public CobrancaService(IHttpClientFactory httpClientFactory, BlazorMockService blazorMockService, DownloadService downloadService)
		{
			_httpClient = httpClientFactory.CreateClient("Api");
			_downloadService = downloadService;
			_httpLocal = httpClientFactory.CreateClient("ApiLocal");
			_mockBlazor = blazorMockService;
		}
		
	
		
		public async Task<HttpResponseMessage> ControleGeralAmortizacaoCaixa()
		{
			var response = await _httpClient.GetAsync($"api/ControleGeralAmortizacaoCaixa");	
			return response;
		}
		public async Task<HttpResponseMessage> ConsultaControlesPendentes()
		{
			var response = await _httpClient.GetAsync($"api/ConsultaControlesPendentes");
	
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarMovimentacoesParaAnalise(int coControle)
		{
			var response = await _httpClient.GetAsync($"api/ConsultarMovimentacoesParaAnalise?coControle={coControle}");
		
			return response;
		}
		public async Task<HttpResponseMessage> ConsultaDrpsDoControle(int coControle)
		{
			var response = await _httpClient.GetAsync($"api/ConsultaDrpsDoControle?coControle={coControle}");	
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarTodasMovimentacoesDoControle(int coControle)
		{		
			var response = await _httpClient.GetAsync($"api/ConsultarTodasMovimentacoesDoControle?coControle={coControle}");	
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarMovimentacoesDeAmortizacao(int coControle)
		{
			var response = await _httpClient.GetAsync($"api/ConsultarMovimentacoesDeAmortizacao?coControle={coControle}");
	
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarMovimentacoesDeDevolucao(int coControle)
		{
			var response = await _httpClient.GetAsync($"api/ConsultarMovimentacoesDeDevolucao?coControle={coControle}");

			return response;
		}
		public async Task<HttpResponseMessage> ConsultarMovimentacoesDeCci(int coControle)
		{
			var response = await _httpClient.GetAsync($"api/ConsultarMovimentacoesDeCci?coControle={coControle}");
			return response;
		}
		public async Task<HttpResponseMessage> ProcessarAmortizacaoCaixa(SenhaUsuario senha)
		{
			var response = await _httpClient.PostAsJsonAsync($"api/ProcessarAmortizacaoCaixa", senha);
			return response;
		}
		public async Task<HttpResponseMessage> CancelarOperacaoAmortizacaoCaixa(SenhaUsuario senha)
		{
			var response = await _httpClient.PostAsJsonAsync($"api/CancelarOperacaoAmortizacaoCaixa", senha);
			return response;
		}
		public async Task<HttpResponseMessage> ResetarMovimentacaoFinanceira(AlteracaoMovimentacaoFinanceira movimentacaoFinanceiraData)
		{
			var response = await _httpClient.PostAsJsonAsync($"api/ResetarMovimentacaoFinanceira",movimentacaoFinanceiraData);	
			return response;
		}
		public async Task<HttpResponseMessage> LancarMovimentacaoFinanceiraTotalEmCci(AlteracaoMovimentacaoFinanceira movimentacaoFinanceiraTotalCciData)
		{
			var response = await _httpClient.PostAsJsonAsync($"api/LancarMovimentacaoFinanceiraTotalEmCci", movimentacaoFinanceiraTotalCciData);
			return response;
		}
		public async Task<HttpResponseMessage> LancarMovimentacaoFinanceiraParcialCci(AlteracaoMovimentacaoFinanceira movimentacaoFinanceiraParcialCciData)
		{
			var response = await _httpClient.PostAsJsonAsync($"api/LancarMovimentacaoFinanceiraParcialCci", movimentacaoFinanceiraParcialCciData);
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarConfiguracoesGerais()
		{
			var response = await _httpClient.GetAsync($"api/ConsultarConfiguracoesGerais");
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarHistoricoDeTms()
		{
			var response = await _httpClient.GetAsync($"api/ConsultarHistoricoDeTms");
			return response;
		}
		public async Task<HttpResponseMessage> AlterarContratoCci(string nuContrato, string nuContratoDv)
		{
			var parametros = new
			{
				nuContrato,
				nuContratoDv
			};

			var response = await _httpClient.PostAsJsonAsync($"api/AlterarContratoCci", parametros);
			return response;
		}
		public async Task<HttpResponseMessage> AlterarContratoCciSacPrice(AlterarContratoCciSacPriceRequest parametros)
		{
			var response = await _httpClient.PostAsJsonAsync($"api/AlterarContratoCciSacPrice", parametros);
			return response;
		}
		public async Task<HttpResponseMessage> AlterarHistoricoTms(int idModelo, string historico)
		{
			AlterarHistoricoTms parametros = new AlterarHistoricoTms()
			{
				IdModelo = idModelo,
				Historico = historico,
			};

			var response = await _httpClient.PostAsJsonAsync($"api/AlterarHistoricoTms", parametros);
			return response;
		}
		public async Task<HttpResponseMessage> AlterarUnidadeDeMovimento(string unidadeMovimento, string UnidadeMovimentoDv)
		{
			var parametros = new
			{
				unidadeMovimento,
				UnidadeMovimentoDv
			};

			var response = await _httpClient.PostAsJsonAsync($"api/AlterarUnidadeDeMovimento", parametros);
			return response;
		}
		public async Task<HttpResponseMessage> ConsultarControleGeral(string data)
		{
			var response = await _httpClient.GetAsync($"api/ConsultarControleGeral?data={data}");
			return response;
		}	
		public async Task<bool> DownloadArquivosAmortizacaoCaixa(string data)
		{

			var response = await _httpClient.PostAsync($"api/DownloadArquivosAmortizacaoCaixa?data={data}", null);
			if (!response.IsSuccessStatusCode) return false;
			var fileBytes = await response.Content.ReadAsByteArrayAsync();
			var fileName = "Arquivos.zip";
			return await _downloadService.DownloadJS(fileBytes, fileName);		
		}
		public async Task<HttpResponseMessage> TesteSignalR()
		{
			var response = await _httpClient.PostAsync($"api/TesteSignalR", new StringContent(""));
			return response;
		}

		public async Task<bool> DownloadDRP(DateTime data)
		{
			var request = await _httpClient.PostAsJsonAsync("api/DownloadDRP", data);
			if (!request.IsSuccessStatusCode) return false;
			var fileBytes = await request.Content.ReadAsByteArrayAsync();			
			var fileName = request.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "DRP.zip";
			return await _downloadService.DownloadJS(fileBytes, fileName);
		}
		public async Task<bool> BaixarExcel(DateTime data)
		{
			var request = await _httpClient.PostAsJsonAsync("api/BaixarExcel", data);
			if (!request.IsSuccessStatusCode) return false;
			var fileBytes = await request.Content.ReadAsByteArrayAsync();
			var fileName = request.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "DadosAMC.xlsx";
			return await _downloadService.DownloadJS(fileBytes, fileName);
		}
	}

}

