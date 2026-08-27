using Microsoft.AspNetCore.Components.Forms;
using PlataformaOperacional.Model.CentralPermissoes;
using PlataformaOperacional.Model.DesembososModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PlataformaOperacional.Service.AplicacaoService;
public class DesembolsosService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Api");

    public async Task<HttpResponseMessage> ProcessarDesembolsos(SenhaUsuario senha) =>
        await _httpClient.PostAsJsonAsync($"api/ProcessarDesembolsos", senha.Senha);

    public async Task<HttpResponseMessage> ProcessarDesembolsosBaixa(DesembolsoAutomacaoDados automacaoDados) =>
    await _httpClient.PostAsJsonAsync($"api/ProcessarDesembolsosBaixa", automacaoDados);

    public async Task<HttpResponseMessage> CriarFluxoDesembolsos(List<DesembolsoFluxoDto> desembolsos) =>
        await _httpClient.PostAsJsonAsync($"api/CriarFluxoDesembolsos", desembolsos);

    public async Task<HttpResponseMessage> CancelarFluxoDesembolsos(int coFluxo) =>
        await _httpClient.PostAsJsonAsync($"api/CancelarFluxoDesembolsos", coFluxo);

    public async Task<HttpResponseMessage> ExpurgarDesembolso(ExpurgarDesembolsoRequest request) =>
        await _httpClient.PostAsJsonAsync($"api/ExpurgarDesembolsosOAF", request);

    public async Task<HttpResponseMessage> ResetarDesembolso(string fid) =>
        await _httpClient.PostAsJsonAsync($"api/ResetarDesembolsosEsteiraOAF", fid);

    public async Task<HttpResponseMessage> ConsultarControlesPendentes() =>
        await _httpClient.GetAsync($"api/ConsultarControlesPendentes");

    public async Task<HttpResponseMessage> ConsultarControlesExecucaoPorMatricula() =>
        await _httpClient.GetAsync($"api/ConsultarControlesExecucaoPorMatricula");

    public async Task<HttpResponseMessage> ConsultarDesembolsosPorFluxo() =>
        await _httpClient.GetAsync($"api/ConsultarDesembolsosPorFluxo");

    public async Task<HttpResponseMessage> ConsultarDesembolsosPendentes() =>
        await _httpClient.GetAsync($"api/ConsultarDesembolsosPendentes");

    public async Task<HttpResponseMessage> ConsultarDesembolsosNaoSelecionados() =>
        await _httpClient.GetAsync($"api/ConsultarDesembolsosNaoSelecionados");

    public async Task<HttpResponseMessage> ConsultarDrpsPendentes() =>
        await _httpClient.GetAsync($"api/ConsultarDrpsPendentes");

    public async Task<HttpResponseMessage> ConsultarDrpsFinalizadas() =>
        await _httpClient.GetAsync($"api/ConsultarDrpsFinalizadas");

    public async Task<HttpResponseMessage> ConsultarDrpsParaBaixa() =>
        await _httpClient.GetAsync($"api/ConsultarDrpsParaBaixa");

    public async Task<HttpResponseMessage> ConsultarConfiguracoesCaixa() =>
        await _httpClient.GetAsync($"api/ConsultarConfiguracoesCaixa");

    public async Task<HttpResponseMessage> ConsultarConfiguracoesOutrosAfs() =>
        await _httpClient.GetAsync($"api/ConsultarConfiguracoesOutrosAfs");

    public async Task<HttpResponseMessage> ConsultarResumoCards() =>
        await _httpClient.GetAsync($"api/ConsultarResumoConsolidado");

    public async Task<HttpResponseMessage> AtualizarListaDesembolsos() =>
        await _httpClient.PostAsync($"api/AtualizarListaDesembolsos", null);

    public async Task<HttpResponseMessage> TratarDesembolso(TratarDesembolso dados) =>
        await _httpClient.PostAsJsonAsync($"api/TratarDesembolsoManualmente", dados);

    public async Task<HttpResponseMessage> ProcessarValidacaoAsync(IBrowserFile arquivo)
    {
        const long maxFileSize = 20 * 1024 * 1024;

        using var formData = new MultipartFormDataContent();

        await using var stream = arquivo.OpenReadStream(maxFileSize);

        using var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(arquivo.ContentType)
                ? "application/octet-stream"
                : arquivo.ContentType
        );

        formData.Add(
            fileContent,
            name: "Arqruivo",
            fileName: arquivo.Name
        );

        return await _httpClient.PostAsync("api/ProcessarValidacao", formData);
    }

    public async Task<HttpResponseMessage> UploadArquivoValidadoAsync(IBrowserFile arquivo)
    {
        const long maxFileSize = 20 * 1024 * 1024;

        using var formData = new MultipartFormDataContent();

        await using var stream = arquivo.OpenReadStream(maxFileSize);

        using var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(arquivo.ContentType)
                ? "application/octet-stream"
                : arquivo.ContentType
        );

        formData.Add(
            fileContent,
            name: "Arqruivo",
            fileName: arquivo.Name
        );

        return await _httpClient.PostAsync("api/UploadArquivoValidado", formData);
    }
}
