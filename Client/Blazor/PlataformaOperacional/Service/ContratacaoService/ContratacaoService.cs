using PlataformaOperacional.Model.ContratatacaoModel;
using System.Net.Http.Json;

namespace PlataformaOperacional.Service.ContratacaoService
{
    public class ContratacaoService
    {
		private readonly HttpClient _httpClient;
		private readonly HttpClient _httpLocal;

		public ContratacaoService(IHttpClientFactory httpClientFactory)
		{
		
			_httpClient = httpClientFactory.CreateClient("Api");
	
			_httpLocal = httpClientFactory.CreateClient("ApiLocal");
		
		}

		public async Task<HttpResponseMessage> ConsultarContratoPorId(string idContrato)
        {        
            return await _httpClient.GetAsync($"api/ConsultaChecklistContratacaoPorContrato?contrato={idContrato}");
        }
        public async Task<HttpResponseMessage> ConsultarResumoGeralChecklist()
        {
            return await _httpClient.GetAsync("api/ConsultarResumoGeralChecklist");
        }
        public async Task<HttpResponseMessage> ConsultarResumoTodosContratos()
        {
            return await _httpClient.GetAsync("api/ConsultarResumoTodosContratos");
        }
        public async Task<HttpResponseMessage> AtualizarCheckList(int? idChecklist, int? resposta)
         {
            var atualizarCheckList = new ContratacaoAtualizarVerificacao
            {
                IdChecklist = idChecklist,
                Resposta = resposta,
            };          
            return await _httpClient.PostAsJsonAsync("api/AtualizarChecklist", atualizarCheckList);

        }
        public async Task<HttpResponseMessage> ConsultarResumoTomadores()
        {
            return await _httpClient.GetAsync("api/ConsultarResumoTomadores");
        }
        public async Task<HttpResponseMessage> ConsultarContratosPorTomador(string codigoTomador)
        {
            return await _httpClient.GetAsync($"api/ConsultarResumoContratosPorTomador?coTomador={codigoTomador}");

        }
        public async Task<HttpResponseMessage> ConsultarRessalvaPorContrato (string numeroDoContrato)
        {
            return await _httpClient.GetAsync($"api/ConsultarRessalvasPorContrato?contrato={numeroDoContrato}");
        }
        public async Task<HttpResponseMessage> CadastrarRessalva(int? idChecklist, int? idChecklistVerificacao, string observacao, string contrato, int? idCategoriaObs)
        {
            var ressalvaPreenchida = new ContratacaoCadastrarRessalva
            {
                IdChecklist = idChecklist,
                IdChecklistVerificacao = idChecklistVerificacao,
                Observacao = observacao,
                Contratao = contrato,
                IdCategoriaObs = idCategoriaObs,
            };
            return await _httpClient.PostAsJsonAsync($"api/CadastrarRessalva", ressalvaPreenchida);

        }

        public async Task<HttpResponseMessage> DesativarRessalva(int idRessalva) 
        {
            return await _httpClient.DeleteAsync($"api/DesativarRessalva?idRessalva={idRessalva}");
        }

        public async Task<HttpResponseMessage> FinalizarRessalva(int idRessalva,bool tratado, string observacaoTramento)
        {
            var ressalvaFinalizada = new ContratacaoFinalizarRessalva
            {
                IdRessalva = idRessalva,
                Tratado = tratado,
                ObservacaoTratamento = observacaoTramento
            };
            return await _httpClient.PostAsJsonAsync($"api/AtualizarRessalva", ressalvaFinalizada);

        }
        public async Task<HttpResponseMessage> FinalizarContrato(string numeroContrato)
        {            
            return await _httpClient.PostAsJsonAsync($"api/FinalizarChecklistContrato", numeroContrato);
        }
		public async Task<HttpResponseMessage> CancelarFinalizacaoContrato(string numeroContrato)
		{
			return await _httpClient.PostAsJsonAsync($"api/CancelarFinalizacaoContrato", numeroContrato);
		}
	}
}


