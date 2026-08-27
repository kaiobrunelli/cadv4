using PlataformaOperacional.Model.Contabilidade;
using System.Net.Http.Json;
using System.Text.Json;

namespace PlataformaOperacional.Service.Contabilidade

{
    public class ContabilidadeService
    {
        private readonly HttpClient _httpClient;
        private readonly DialogServicePlataformaOperacional _dialogServicePlataformaOperacional;  

        public ContabilidadeService (HttpClient httpClient,DialogServicePlataformaOperacional dialogServicePlataformaOperacional)
        {
            _httpClient = httpClient;
            _dialogServicePlataformaOperacional = dialogServicePlataformaOperacional;        
        }          

      
        public async Task<List<ConsultaLotes>> ControleLotesMovimentacoesContabeis()
        {
       
            var response = await _httpClient.GetFromJsonAsync<List<ConsultaLotes>>("api/ControleLotesMovimentacoesContabeis");
            if (response==null)
            {
                throw new Exception($"Movimentações Contábeis inválidas.");
            }        
            return response;
        }
        public async Task<ConsultaLotes> ConsultaLotePorNumero(int numeroLoteRequest)
        {
            var response = await _httpClient.GetFromJsonAsync<ConsultaLotes>($"api/ConsultaLotePorNumero/{numeroLoteRequest}");
            if (response ==null)
            {
                throw new Exception($"Erro ao consutar numero do lote.");      
            }    
            return response;

        }

        public async Task<List<ConsultaLotes>> ConsultaLotePorMatricula(string matriculaRequest)
        {
            var response = await _httpClient.GetFromJsonAsync<List<ConsultaLotes>>($"api/ConsultaLotesPorMatricula/{matriculaRequest}");
            if (response == null)
            {
                throw new Exception($"Matrícula inválida.");
            }
            return response;
        }

        public async Task<List<ConsultaLotes>> ConsultaLoteEmAberto(string matriculaRequest)
        {
            var response = await _httpClient.GetFromJsonAsync<List<ConsultaLotes>>($"api/ConsultaLoteEmAberto/{matriculaRequest}");
            if (response == null)
            {
                throw new Exception($"Consulta lote em aberto inválida.");
            }
            return response;
        }

        public async Task<string> CapturaMovimentacoesContabeis(MovimentacaoContabil movimentacaoContabil)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync<MovimentacaoContabil>("api/CapturaMovimentacoesContabeis", movimentacaoContabil);
            if (response == null)
            {
                throw new HttpRequestException($"Erro ao solicitar movimentação  contábil: {response!.StatusCode}.");
            }            
            return await response.Content.ReadAsStringAsync();

        }


        public async Task AtualizarLote(int idParcelaContabilidade)
        {
            await _dialogServicePlataformaOperacional.OpenDialogAsync();
        }
        public async Task BaixarLote(int idParcelaContabilidade)
        {
            await _dialogServicePlataformaOperacional.OpenDialogAsync();
        }
        public async Task RemoverLote(int idParcelaContabilidade)
        {
            await _dialogServicePlataformaOperacional.OpenDialogAsync();
        }
    }
}
