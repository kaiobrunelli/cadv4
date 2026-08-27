namespace PlataformaOperacional.Service.Middleware
{
	public class PlataformaService
	{
		private readonly HttpClient _httpClient;
		private readonly HttpClient _httpLocal;
		private readonly BlazorMockService _mockBlazor;

		public PlataformaService(IHttpClientFactory httpClientFactory, BlazorMockService blazorMockService)
		{
			_httpClient = httpClientFactory.CreateClient("Api");

			_httpLocal = httpClientFactory.CreateClient("ApiLocal");
			_mockBlazor = blazorMockService;
		}

		public Dictionary<string, string> ConsultarBaseAddres()
		{
			return new Dictionary<string, string>
			{
				{ "Api", _httpClient.BaseAddress.AbsoluteUri.ToString() ?? "Não identificada" },
				{ "Api Local", _httpLocal.BaseAddress.AbsoluteUri.ToString() ?? "Não identificada" },
				{ "Mock de dados", _mockBlazor.MockarDados ? "Ativado" : "Desativado" },
			};
		}
	}
}
