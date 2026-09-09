using ControleAnaliseDesembolso.Componentes;
using ControleAnaliseDesembolso.Modelos;
using System.Net.Http.Json;

namespace ControleAnaliseDesembolso.Service;

public class ControleAnaliseDesembolsoService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Api");

    public static async Task<string> ExtrairMensagemDeErroAsync(HttpResponseMessage resposta)
    {
        var corpo = await resposta.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(corpo))
        {
            try
            {
                var problema = System.Text.Json.JsonSerializer.Deserialize<ProblemDetailsDto>(corpo,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (!string.IsNullOrWhiteSpace(problema?.Detail)) return problema.Detail;
                if (!string.IsNullOrWhiteSpace(problema?.Title)) return problema.Title;
            }
            catch (System.Text.Json.JsonException)
            {
            }
        }

        return $"Falha ao processar (código {(int)resposta.StatusCode}). Tente novamente.";
    }

    private class ProblemDetailsDto
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
    }

    public async Task<List<DesembolsoCAD>> ObterTodosAsync()
    {
        var lista = await _httpClient.GetFromJsonAsync<List<DesembolsoResponseDto>>("api/ObterTodosDesembolsos");
        return (lista ?? []).Select(MapearParaDesembolsoCAD).ToList();
    }

    public async Task<DesembolsoCAD?> ObterPorIdAsync(string id)
    {
        var todos = await ObterTodosAsync();
        return todos.FirstOrDefault(d => d.Id == id);
    }

    public async Task<DesembolsoDetalheDto?> ObterDetalheDesembolsoAsync(string id) =>
        await _httpClient.GetFromJsonAsync<DesembolsoDetalheDto>($"api/ObterDetalheDesembolso/{id}");

    public static string MapearStatus(int statusServidor) => statusServidor switch
    {
        1 => "pendencia",
        2 => "pendente",
        3 => "aprovado",
        5 => "baixado",
        4 => "negado",
        6 => "cancelado",
        _ => "pendencia",
    };

    private static DesembolsoCAD MapearParaDesembolsoCAD(DesembolsoResponseDto d) => new()
    {
        Id = d.CoControleDesembolso.ToString(),
        NumId = d.NumId,
        Contrato = d.Contrato,
        Mutuario = d.Mutuario,
        Gigov = d.Gigov,
        Valor = d.Valor,
        AgenteFinanceiro = d.AgenteFinanceiro,
        AgentePromotor = d.AgentePromotor,
        MatriculaSolicitante = d.MatriculaSolicitante,
        Fase = "",
        ValidacoesOk = d.ValidacoesOk,
        ValidacoesTotal = d.ValidacoesTotal,
        Status = MapearStatus(d.Status),
        PrazoFinal = d.PrazoFinal,
        DtSolicitado = d.DtSolicitado,
        DtConclusao = d.DtConclusao,
        PrimeiroDesembolso = d.PrimeiroDesembolso,
        UltimoDesembolso = d.UltimoDesembolso,
        Adiantamento = d.Adiantamento,
        Recorrente = d.Recorrente,
        Sanepar = d.Sanepar,
        ContratoAo = d.ContratoAo,
        ContratoAoDv = d.ContratoAoDv,
        ResponsavelAnalise = d.ResponsavelAnalise,
    };

    public async Task<List<string>> ObterGigovDoUsuarioAsync(string matricula)
    {
        try
        {
            var lista = await _httpClient.GetFromJsonAsync<List<string>>($"api/ObterCodigosGigovPorMatricula?matricula={Uri.EscapeDataString(matricula)}");
            return lista ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CAD] Falha ao obter GIGOV do usuário: {ex.Message}");
            return [];
        }
    }

    // A partir daqui: a matrícula de quem está agindo não é mais enviada pelo
    // client — quem identifica é a plataforma (IPlataformaOperacionalService,
    // no backend), a partir da senha informada aqui. "senha" sempre vai como
    // query string (?senha=...), nunca no corpo.

    public async Task<HttpResponseMessage> AprovarAsync(string id, string usuarioNome, string senha)
    {
        var req = new { UsuarioNome = usuarioNome };
        return await _httpClient.PutAsJsonAsync($"api/Aprovar/{id}?senha={Uri.EscapeDataString(senha)}", req);
    }

    public async Task<bool> RejeitarAsync(string id, string usuarioNome, string codigoCoordenacao, string senha, string justificativa = "")
    {
        var req = new { UsuarioNome = usuarioNome, CodigoCoordenacao = codigoCoordenacao, Justificativa = justificativa };
        var resposta = await _httpClient.PutAsJsonAsync($"api/Rejeitar/{id}?senha={Uri.EscapeDataString(senha)}", req);
        return resposta.IsSuccessStatusCode;
    }

    public async Task<bool> CancelarAsync(string id, string usuarioNome, string motivo, string senha)
    {
        var req = new { UsuarioNome = usuarioNome, Motivo = motivo };
        var resposta = await _httpClient.PutAsJsonAsync($"api/Cancelar/{id}?senha={Uri.EscapeDataString(senha)}", req);
        return resposta.IsSuccessStatusCode;
    }

    public async Task<bool> AtualizarMensagemCefgaAsync(string id, string mensagemCefga, string senha)
    {
        var req = new { MensagemCefga = mensagemCefga };
        var resposta = await _httpClient.PutAsJsonAsync($"api/AtualizarMensagemCefga/{id}?senha={Uri.EscapeDataString(senha)}", req);
        return resposta.IsSuccessStatusCode;
    }

    public async Task<List<ConferenciaCampoDto>> ExecutarConferenciaCamposAsync(string id, string senha)
    {
        var resposta = await _httpClient.PostAsync($"api/ExecutarConferenciaCampos/{id}?senha={Uri.EscapeDataString(senha)}", null);
        if (!resposta.IsSuccessStatusCode) return [];
        return await resposta.Content.ReadFromJsonAsync<List<ConferenciaCampoDto>>() ?? [];
    }

    public async Task<bool> VincularAnalistaAsync(string id, string? matriculaAnalista, string senha)
    {
        var resposta = await _httpClient.PutAsJsonAsync($"api/VincularResponsavel/{id}?senha={Uri.EscapeDataString(senha)}", matriculaAnalista);
        return resposta.IsSuccessStatusCode;
    }

    public async Task<bool> RemoverVinculoAsync(string id, string senha)
    {
        var resposta = await _httpClient.PutAsync($"api/RemoverResponsavel/{id}?senha={Uri.EscapeDataString(senha)}", null);
        return resposta.IsSuccessStatusCode;
    }

    public async Task<List<Funcionario>> ObterAnalistasAsync(string codigoCoordenacao)
    {
        var lista = await _httpClient.GetFromJsonAsync<List<Funcionario>>(
            $"api/ObterEmpregadosPorCoordenacao?coordenacao={Uri.EscapeDataString(codigoCoordenacao)}");
        return lista ?? [];
    }

    public async Task<bool> ValidarAsync(string id, string senha)
    {
        var resposta = await _httpClient.PostAsync($"api/Validar/{id}?senha={Uri.EscapeDataString(senha)}", null);
        return resposta.IsSuccessStatusCode;
    }

    public async Task<bool> ValidarTodosPendentesAsync(string senha)
    {
        var resposta = await _httpClient.PostAsync($"api/ValidarTodosPendentes?senha={Uri.EscapeDataString(senha)}", null);
        return resposta.IsSuccessStatusCode;
    }

    public async Task<List<ValidacaoTemplateDto>> ObterValidacoesTemplateAsync()
    {
        var lista = await _httpClient.GetFromJsonAsync<List<ValidacaoTemplateDto>>("api/ObterValidacoesTemplate");
        return lista ?? [];
    }

    public async Task<HttpResponseMessage> AdicionarComentarioAsync(
        string coDesembolso, int coValidacao, string texto, string tipoRegistro, string matriculaAutor, string nomeAutor, int unidadeAutor, string senha)
    {
        var req = new { DeMensagem = texto, TipoMensagem = tipoRegistro, MatriculaAutor = matriculaAutor, NomeAutor = nomeAutor, UnidadeAutor = unidadeAutor };
        return await _httpClient.PostAsJsonAsync(
            $"api/AdicionarComentario?coControleDesembolso={coDesembolso}&coValidacao={coValidacao}&senha={Uri.EscapeDataString(senha)}", req);
    }

    public async Task<HttpResponseMessage> EditarComentarioAsync(
        string coDesembolso, int coValidacao, int comentarioId, string novoTexto, string matriculaSolicitante, string senha)
    {
        var req = new { DeMensagem = novoTexto, MatriculaSolicitante = matriculaSolicitante };
        return await _httpClient.PutAsJsonAsync($"api/EditarComentario/{comentarioId}?senha={Uri.EscapeDataString(senha)}", req);
    }

    public async Task<HttpResponseMessage> RemoverComentarioAsync(
        string coDesembolso, int coValidacao, int coComentario, string matriculaSolicitante, string senha)
    {
        return await _httpClient.PutAsync(
            $"api/RemoverComentario/{coComentario}?matriculaSolicitante={Uri.EscapeDataString(matriculaSolicitante)}&senha={Uri.EscapeDataString(senha)}",
            null);
    }

    public async Task<HttpResponseMessage> CriarFichaPedidoDesembolsoAsync(object request, string senha) =>
        await _httpClient.PostAsJsonAsync($"api/CriarFichaPedidoDesembolso?senha={Uri.EscapeDataString(senha)}", request);

    public async Task<HttpResponseMessage> ReenviarFichaAsync(int coFpd, object request, string senha) =>
        await _httpClient.PutAsJsonAsync($"api/ReenviarFicha/{coFpd}?senha={Uri.EscapeDataString(senha)}", request);

    public async Task<bool> ExecutarProcessamentoAsync()
    {
        var resposta = await _httpClient.PostAsync("api/processamento/executar", null);
        return resposta.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> BuscarContratoAFAsync(string coContratoAF, string coContratoAFDV) =>
        await _httpClient.GetAsync($"api/SolicitarDadosFpd?CoContratoAf={coContratoAF}&CoContratoAfDv={coContratoAFDV}");

    public async Task<List<RegistroDrp>> ObterRegistrosDrpAsync()
    {
        var lista = await _httpClient.GetFromJsonAsync<List<RegistroDrp>>("api/ObterRegistrosDrp");
        return lista ?? [];
    }

    public async Task<bool> BaixarDrpAsync(List<int> ids, string senha)
    {
        var req = new { Ids = ids };
        var resposta = await _httpClient.PutAsJsonAsync($"api/BaixarDrp?senha={Uri.EscapeDataString(senha)}", req);
        return resposta.IsSuccessStatusCode;
    }

}
