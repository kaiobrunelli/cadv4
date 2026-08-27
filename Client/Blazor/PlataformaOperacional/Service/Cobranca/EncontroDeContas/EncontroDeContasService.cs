using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Extensions;
using PlataformaOperacional.Model.CentralPermissoes;
using PlataformaOperacional.Model.Cobranca.EncontroDeContas;
using PlataformaOperacional.Model.Plataforma;
using PlataformaOperacional.Model.Shared;
using PlataformaOperacional.Pages.Aplicacao.Preditor.PreditorComponents;
using PlataformaOperacional.Service.Middleware;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;


namespace PlataformaOperacional.Service.Cobranca.EncontroDeContas
{
    public  class EncontroDeContasService
    {
       

        #region Propriedades

      
        public int TotalDeFases { get; set; }
        private bool _signalREscutaIniciada = false;
        private bool _execucaoFinalizada = false;
        private Func<ObservadorAutomacao, Task>? _callbackTesteSignalR;
        public ObservadorAutomacao ObservadorAutomacaoAPI = new ObservadorAutomacao();
        private readonly Dictionary<string, DateTime> _ultimoAlerta = new();
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpLocal;
        private readonly BlazorMockService _mockBlazor;
        private readonly SignalRService _signalRService;
        private DownloadService _downloadService;
        private const string ChaveSignalR = "ProcessarEncontroDeContas";
        public int NumeroFaseAtualProgresso { get; set; } = -1;
        public bool EmExecucaoFase => IsRunning &&
               ObservadorAutomacaoAPI.PercentualProcessado > 0 &&
               ObservadorAutomacaoAPI.PercentualProcessado < 100;
        public bool ExecutandoSP { get; private set; }
        public bool ErroSP { get; private set; }
        public string MensagemErroSP { get; private set; } = "";

       
        private CancellationTokenSource? _cts;
        public List<Contract> Contratos { get; }
        public DadosRelatorioTabela DadosRelatorio { get; private set; }
        public FasesDeExecucao FaseAtual { get; private set; } = FasesDeExecucao.Selecao;
        public bool IsRunning { get; private set; }
        public bool MostrarProgressBar { get; private set; }
        public bool UsarLista { get; set; } = false;

        public List<AlertaItem> Alertas { get; } = new();


        public IReadOnlyList<AlertaItem> AlertasCriticos =>
            Alertas.Where(a => a.TipoAlerta == TipoDeAlerta.Err
                            || a.TipoAlerta == TipoDeAlerta.Warn).ToList();
        public IReadOnlyList<AlertaItem> AlertasInformativos =>
            Alertas.Where(a => a.TipoAlerta == TipoDeAlerta.Ok
                            || a.TipoAlerta == TipoDeAlerta.Info).ToList();

        public bool TemAlertaCritico => AlertasCriticos.Count > 0;
        public double ProgressoPercentual { get; private set; }
        public int EtapaAtual { get; private set; }
        public int TotalEtapas { get; private set; }
        public string DescricaoEtapa { get; private set; } = "";
        public string NomeProcesso { get; private set; } = "";

        public event Action? StateChanged;
        public int TotalProcessadoFlag { get; private set; }
        private SimulacaoA _simulacaoA = new();
        private SimulacaoB _simulacaoB = new();
        public EncontroDeContasService(
            IOptions<SimulacaoA> simulacaoA,
            IOptions<SimulacaoB> simulacaoB,
           IHttpClientFactory httpClientFactory,
           BlazorMockService blazorMockService,
           DownloadService downloadService,
           SignalRService signalRService)
        {
            _simulacaoA = simulacaoA.Value;
            _simulacaoB = simulacaoB.Value;
            _httpClient = httpClientFactory.CreateClient("Api");
            _downloadService = downloadService;
            _httpLocal = httpClientFactory.CreateClient("ApiLocal");
            _mockBlazor = blazorMockService;
            _signalRService = signalRService;

            Contratos = new List<Contract>();
        }


        #endregion

        private void Notify() => StateChanged?.Invoke();

        public void AddContract(string number, TipoDeMovimentacao type)
        {
            if (string.IsNullOrWhiteSpace(number) || !UsarLista) return;
            var c = new Contract
            {
                Number = number.Trim(),
                Type = type,
                Holder = new Contract().HolderFor(type),
                Status = ContractStatus.Ready
            };
            Contratos.Add(c);
            DadosRelatorio.Reaberturas = Contratos.Count;

            Notify();
        }

        public void RemoveContract(string id)
        {
            var c = Contratos.FirstOrDefault(x => x.Id == id);
            if (c == null || IsRunning) return;
            Contratos.Remove(c);
            DadosRelatorio.Reaberturas = Contratos.Count;
            Notify();
        }

        public void FecharAlerta(string id)
        {
            Alertas.RemoveAll(a => a.Id == id);
            Notify();
        }

        public void PushAlert(TipoDeAlerta tipo, string titulo, string? desc = null)
        {
            Alertas.Add(new AlertaItem { TipoAlerta = tipo, Titulo = titulo, Desc = desc });
            Notify();
        }
       

        public async Task SimularProgressBarSP()
        {        
        
          
            if (_simulacaoA.ExecutandoSP)
            {
                await OnProgressoSignalR(new ObservadorAutomacao
                {
                    ChaveConexao = ChaveSignalR,
                    NomeProcesso = _simulacaoA.NomeProcesso,
                    Severity = _simulacaoA.Severity,
                    PercentualProcessado = _simulacaoA.PercentualProcessado,
                    ExecutandoSP = _simulacaoA.ExecutandoSP,
                    NumeroFaseAtual = _simulacaoA.NumeroFaseAtual,
                    TotalProcessado = _simulacaoA.TotalProcessado,
                    Mensagem = _simulacaoA.Mensagem,
                });
                await Task.Delay(4000); 
            }
            else
            {
           
                for (int i = 0; i < 101; i++)
                {
                    await Task.Delay(75);
                    await OnProgressoSignalR(new ObservadorAutomacao
                    {
                        ChaveConexao = ChaveSignalR,
                        NomeProcesso = _simulacaoA.NomeProcesso,
                        Severity = Severity.Normal.ToString(),
                        PercentualProcessado = i,
                        TotalAProcessar = 100,
                        ExecutandoSP = _simulacaoA.ExecutandoSP,
                        NumeroFaseAtual = _simulacaoA.NumeroFaseAtual,
                        TotalProcessado = i,
                        Mensagem = _simulacaoA.Mensagem,
                    });
                }
            }
      
            if (_simulacaoB.ExecutandoSP)
            {
                await OnProgressoSignalR(new ObservadorAutomacao
                {
                    ChaveConexao = ChaveSignalR,
                    NomeProcesso = _simulacaoB.NomeProcesso,
                    Severity = _simulacaoB.Severity,
                    PercentualProcessado = _simulacaoB.PercentualProcessado,
                    ExecutandoSP = _simulacaoB.ExecutandoSP,
                    NumeroFaseAtual = _simulacaoB.NumeroFaseAtual,
                    TotalProcessado = _simulacaoB.TotalProcessado,
                    Mensagem = _simulacaoB.Mensagem,
                });
                await Task.Delay(3000);
            }
            else
            {
            
                for (int i = 0; i < 101; i++)
                {
                    await Task.Delay(75);
                  await OnProgressoSignalR(new ObservadorAutomacao
                  {
                      ExecutandoSP = false,
                      NomeProcesso = _simulacaoB.NomeProcesso,
                      NumeroFaseAtual = _simulacaoB.NumeroFaseAtual,
                      TotalAProcessar = _simulacaoB.TotalAProcessar,
                      PercentualProcessado = i,
                      TotalProcessado = i,
                      Severity = _simulacaoB.Severity,
                      Mensagem = _simulacaoB.Mensagem,
                  });
                
                }
            }

            await Task.Delay(4000);















        }


        public async Task IniciarEscutaSignalR()
        {
            if (_signalREscutaIniciada) return;

            _callbackTesteSignalR = OnProgressoSignalR;
            await _signalRService.IniciarEscutaDaOperacao(ChaveSignalR);
            _signalRService.RegistrarObserver(ChaveSignalR, _callbackTesteSignalR);
            _signalREscutaIniciada = true;
        }

        private readonly TimeSpan _tempo = TimeSpan.FromSeconds(2);
        private async Task OnProgressoSignalR(ObservadorAutomacao obs)
        {
            ObservadorAutomacaoAPI = obs;
            NomeProcesso = obs.NomeProcesso;
            var testeXp = obs.Severity;
            if (ExecutandoSP && obs.AlertaSeverity == Severity.Error)
            {
                ErroSP = true;
                MensagemErroSP = obs.Mensagem;
                PushAlert(TipoDeAlerta.Err, $"Processo: {obs.NomeProcesso} | {obs.Mensagem}");
                Notify();
                return;
            }
          

            ErroSP = false;
            MensagemErroSP = "";


            AlertasSeverity(obs);


            ExecutandoSP = obs.ExecutandoSP;
            if (obs.ExecutandoSP)
            {
                DescricaoEtapa = obs.Mensagem;
                if (!IsRunning) IsRunning = true;
                TotalProcessadoFlag = 100;
                Notify();
                return;
            }


            if (_execucaoFinalizada) return;

            var pct = obs.TotalAProcessar > 0
                ? Math.Clamp((double)obs.TotalProcessado / obs.TotalAProcessar * 100.0, 0, 100)
                : 0;

            var concluiu = TotalDeFases > 0 && obs.NumeroFaseAtual > TotalDeFases;
            NumeroFaseAtualProgresso = obs.NumeroFaseAtual;
            ProgressoPercentual = concluiu ? 100 : pct;
            EtapaAtual = obs.TotalProcessado;
            TotalEtapas = obs.TotalAProcessar;
            DescricaoEtapa = string.IsNullOrWhiteSpace(obs.Mensagem)
                ? $"Processando {obs.TotalProcessado} de {obs.TotalAProcessar}…"
                : obs.Mensagem;
            if (pct == 100)
            {
                _execucaoFinalizada = true;
                FaseAtual = FasesDeExecucao.Conclusao;
                await Task.Delay(3000);
                IsRunning = false;
                MostrarProgressBar = false;
                Notify();
                return;
            }
            if (!IsRunning) IsRunning = true;
            FaseAtual = FasesDeExecucao.Execucao;
         
            Notify();
        }

        public async Task<HttpResponseMessage> ExecutarTesteSignalR()
        {
            _execucaoFinalizada = false;
            IsRunning = true;
            FaseAtual = FasesDeExecucao.Execucao;
            ProgressoPercentual = 0;
            EtapaAtual = 0;
            DescricaoEtapa = "Iniciando teste SignalR…";
            Alertas.Clear();
            Notify();
            return await _httpClient.PostAsync("api/TesteSignalR", null);
        }

        public async Task<HttpResponseMessage> ControleGeralEncontroDeContas()
        {
            var response = await _httpClient.GetAsync("api/ControleGeralEncontroDeContas");
            if (!response.IsSuccessStatusCode)
            {
                IsRunning = false;
                PushAlert(TipoDeAlerta.Err, "Falha ", "Não foi possível carregar Controle Geral.");
                Notify();
            }
            return response;
        }

        public async Task<HttpResponseMessage> ConsultaControlesPendentes()
        {
            var response = await _httpClient.GetAsync($"api/ConsultaControlesPendentes");
            return response;
        }

        public async Task<HttpResponseMessage> ProcessarEncontroDeContas(SenhaUsuario senha)
        {
            await IniciarEscutaSignalR();
            _execucaoFinalizada = false;
            IsRunning = true;
            FaseAtual = FasesDeExecucao.Execucao;
            ProgressoPercentual = 0;
            EtapaAtual = 0;
            DescricaoEtapa = "Iniciando processamento Encontro de Contas...";
            Alertas.Clear();
            Notify();
            var response = await _httpClient.PostAsJsonAsync($"api/ProcessarEncontroDeContas", senha);
            if (!response.IsSuccessStatusCode)
            {
                IsRunning = false;
                PushAlert(TipoDeAlerta.Err, "Falha ao iniciar", "Não foi possível inciar o processamento.");
                Notify();
            }
            return response;
        }

        public async Task<HttpResponseMessage> CancelarExecucao(SenhaUsuario senha)
        {
            var response = await _httpClient.PostAsJsonAsync("api/CancelarEncontroDeContas", senha);
            return response;
        }
        public void AlertasSeverity(ObservadorAutomacao obs)
        {
            if (obs.AlertaSeverity == Severity.Normal) return;

            var chave = $"{obs.AlertaSeverity}:{obs.NomeProcesso}:{obs.Mensagem.Trim().ToLowerInvariant()}";
            var agora = DateTime.UtcNow;

            if (_ultimoAlerta.TryGetValue(chave, out var ultimo) && agora - ultimo < _tempo)
                return;

            _ultimoAlerta[chave] = agora;

            foreach (var k in _ultimoAlerta
                .Where(x => agora - x.Value > _tempo)
                .Select(x => x.Key)
                .ToList())
            {
                _ultimoAlerta.Remove(k);
            }

            if (obs.AlertaSeverity == Severity.Error)
            {
                PushAlert(TipoDeAlerta.Err, $"Processo: {obs.NomeProcesso} | {obs.Mensagem}");
                DescricaoEtapa = "Atencao: verifique as notificacoes";
            }
            else if (obs.AlertaSeverity == Severity.Success)
            {
                PushAlert(TipoDeAlerta.Ok, $"Processo: {obs.NomeProcesso} | {obs.Mensagem}");
            }
            else if (obs.AlertaSeverity == Severity.Warning)
            {
                PushAlert(TipoDeAlerta.Warn, $"Processo: {obs.NomeProcesso} | {obs.Mensagem}");
                DescricaoEtapa = "Atencao: verifique as notificacoes";
            }

            Notify();
        }






        public void AlertaControleGeral(List<PlataformaOperacionalAlerta> alertas)
        {
            foreach (PlataformaOperacionalAlerta alerta in alertas)
            {
                if (alerta.AlertaSeverity == Severity.Error)
                {
                    PushAlert(TipoDeAlerta.Err, $"{alerta.MensagemDeAlerta}");
                    Notify();
                }
                if (alerta.AlertaSeverity == Severity.Success)
                {
                    PushAlert(TipoDeAlerta.Ok, $"{alerta.MensagemDeAlerta}");
                    Notify();
                }
                if (alerta.AlertaSeverity == Severity.Warning)
                {
                    PushAlert(TipoDeAlerta.Warn, $"{alerta.MensagemDeAlerta}");
                    Notify();
                }
            }

        }

    }
}






