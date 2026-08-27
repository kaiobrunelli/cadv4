using Microsoft.AspNetCore.SignalR.Client;
using PlataformaOperacional.Model.Plataforma;
using System.Collections.Concurrent;

namespace PlataformaOperacional.Service.Middleware
{
    public class SignalRService : IAsyncDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpLocal;
        private readonly BlazorMockService _mockBlazor;

        private HubConnection _hubConnection;
        private string _hubUrlProd;
        private string _matricula;
        private bool _descartado;

        private readonly SemaphoreSlim _mutexConexao = new(1, 1);
        private readonly object _lockConstrucao = new();

        // O chatHub só existe no backend real (ControleAnaliseDesembolso.Api, "Api" client —
        // http://localhost:5079 em dev). "ApiLocal" aponta pra origem estática do próprio WASM
        // (wwwroot), que não tem hub nenhum mapeado — por isso não pode depender de MockarDados
        // aqui (isso é só pra escolher a origem dos dados de usuário mockado, não do SignalR).
        public string _baseAdress => _httpClient.BaseAddress.ToString();
        public string HubUrlProd => _hubUrlProd;
        public string UsuarioAtual => _matricula;
        public bool Conectado => _hubConnection?.State == HubConnectionState.Connected;


        private const string EventoProgresso = "ProgressoProcessamento";
        public event Action<ObservadorAutomacao> AoReceberProgresso;

        public event Action AoConectar;
        public event Action<string> AoReconectar;
        public event Action AoDesconectar;

        public SignalRService(
       IHttpClientFactory httpClientFactory,
       BlazorMockService blazorMockService)
        {
            _httpClient = httpClientFactory.CreateClient("Api");
            _httpLocal = httpClientFactory.CreateClient("ApiLocal");
            _mockBlazor = blazorMockService;

            CriarHubUrl(_baseAdress);

            _fabricasEscuta[EventoProgresso] = conexao =>
                conexao.On<int, ObservadorAutomacao>(
                    EventoProgresso,
                    (_, observador) =>
                    {
                        AoReceberProgresso?.Invoke(observador);
                    });
        }


        private readonly ConcurrentDictionary<string, IDisposable> _registeredKeys = new();

        private readonly ConcurrentDictionary<string, Func<HubConnection, IDisposable>> _fabricasEscuta = new();

        private readonly ConcurrentDictionary<string, ObservadorAutomacao> _progressoAtualPorHub = new();

        private readonly ConcurrentDictionary<string, List<Func<ObservadorAutomacao, Task>>> _observersPorChave = new();

        private readonly ConcurrentDictionary<string, bool> _flagCompletouPorHub = new();

        public event Action<bool> OnProgressUpdateCompleted;
        public event Action<string> OnProgressUpdateCompletedByKey;

        public void CriarHubUrl(string url)
        {
            _hubUrlProd = $"{url}chatHub";
        }


        public async Task DefinirUsuarioAsync(string matricula)
        {
            if (string.Equals(_matricula, matricula, StringComparison.Ordinal))
                return;

            _matricula = matricula;

            if (_hubConnection is not null)
                await ReconstruirConexaoAsync();
        }


        public async Task IniciarHubConnection()
        {
            if (Conectado || _descartado)
                return;

            await _mutexConexao.WaitAsync();
            try
            {
                if (Conectado || _descartado)
                    return;

                GarantirConexaoConstruida();

                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
                    AoConectar?.Invoke();
                }
            }
            catch
            {
                AgendarNovaTentativa();
                throw;
            }
            finally
            {
                _mutexConexao.Release();
            }
        }

        private void GarantirConexaoConstruida()
        {
            if (_hubConnection is not null) return;
            lock (_lockConstrucao)
            {
                _hubConnection ??= ConstruirConexao();
            }
        }

        private HubConnection ConstruirConexao()
        {
            var url = string.IsNullOrWhiteSpace(_matricula)
                ? _hubUrlProd
                : $"{_hubUrlProd}?userId={Uri.EscapeDataString(_matricula)}";

            var conexao = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect(new RepetirSempreRetryPolicy())
                .Build();

            conexao.Reconnecting += _ =>
            {
                AoDesconectar?.Invoke();
                return Task.CompletedTask;
            };

            conexao.Reconnected += connectionId =>
            {
                AoReconectar?.Invoke(connectionId);
                return Task.CompletedTask;
            };

            conexao.Closed += _ =>
            {
                AoDesconectar?.Invoke();
                if (!_descartado)
                    AgendarNovaTentativa();
                return Task.CompletedTask;
            };

            return conexao;
        }

        private void AgendarNovaTentativa()
        {
            _ = Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(async _ =>
            {
                if (_descartado || Conectado) return;
                try
                {
                    await IniciarHubConnection();
                }
                catch
                {
                }
            });
        }

        private async Task ReconstruirConexaoAsync()
        {
            await _mutexConexao.WaitAsync();
            try
            {
                foreach (var assinatura in _registeredKeys.Values)
                    assinatura.Dispose();
                _registeredKeys.Clear();

                if (_hubConnection is not null)
                {
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                }

                GarantirConexaoConstruida();

                foreach (var fabrica in _fabricasEscuta)
                    _registeredKeys.GetOrAdd(fabrica.Key, _ => fabrica.Value(_hubConnection));
            }
            finally
            {
                _mutexConexao.Release();
            }

            await IniciarHubConnection();
        }


        public Task EscutarEvento<T>(string nomeEvento, Func<T, Task> handler)
            => RegistrarEscutaAsync(nomeEvento, conexao => conexao.On<T>(nomeEvento, handler));

        private async Task RegistrarEscutaAsync(string nomeEvento, Func<HubConnection, IDisposable> fabrica)
        {
            if (_registeredKeys.ContainsKey(nomeEvento))
                return;

            _fabricasEscuta[nomeEvento] = fabrica;

            GarantirConexaoConstruida();
            _registeredKeys.GetOrAdd(nomeEvento, _ => fabrica(_hubConnection));

            await IniciarHubConnection();
        }


        public void RegistrarObserver(string chave, Func<ObservadorAutomacao, Task> callback)
        {
            _observersPorChave.AddOrUpdate(chave,
                new List<Func<ObservadorAutomacao, Task>> { callback },
                (key, list) =>
                {
                    lock (list)
                    {
                        if (!list.Contains(callback))
                        {
                            list.Add(callback);
                        }
                    }
                    return list;
                });

            if (_progressoAtualPorHub.TryGetValue(chave, out var ultimoEstado))
            {
                _ = callback.Invoke(ultimoEstado);
            }
        }

        public void RemoverObserver(string chave, Func<ObservadorAutomacao, Task> callback)
        {
            if (_observersPorChave.TryGetValue(chave, out var list))
            {
                lock (list)
                {
                    list.Remove(callback);
                }
            }
        }

        public Task IniciarEscutaDaOperacao(string hubConnectId)
            => RegistrarEscutaAsync(hubConnectId, conexao =>
                conexao.On<int, ObservadorAutomacao>(hubConnectId, async (progresso, observer) =>
                {
                    _progressoAtualPorHub[hubConnectId] = observer;

                    if (_observersPorChave.TryGetValue(hubConnectId, out var callbacksList))
                    {
                        Func<ObservadorAutomacao, Task>[] callbacksSnapshot;

                        lock (callbacksList)
                        {
                            callbacksSnapshot = callbacksList.ToArray();
                        }

                        if (callbacksSnapshot.Length > 0)
                        {
                            await Task.WhenAll(callbacksSnapshot.Select(cb => cb(observer)));
                        }
                    }

                    if (observer.PercentualProcessado == 100 && !_flagCompletouPorHub.GetValueOrDefault(hubConnectId))
                    {
                        _flagCompletouPorHub[hubConnectId] = true;
                        OnProgressUpdateCompleted?.Invoke(false);
                        OnProgressUpdateCompletedByKey?.Invoke(hubConnectId);
                    }
                }));

        public void InterromperEscutaDaOperacao(string hubConnectId)
        {
            if (_registeredKeys.TryRemove(hubConnectId, out var subscription))
            {
                subscription.Dispose();

                _fabricasEscuta.TryRemove(hubConnectId, out _);

                _progressoAtualPorHub.TryRemove(hubConnectId, out _);
                _flagCompletouPorHub.TryRemove(hubConnectId, out _);
            }
        }

        public async Task ReiniciarEscutaDaOperacao(string hubConnectId)
        {
            InterromperEscutaDaOperacao(hubConnectId);
            await IniciarEscutaDaOperacao(hubConnectId);
        }

        public Task<ObservadorAutomacao?> ObterEstadoAtual(string hubConnectId)
        {
            if (_progressoAtualPorHub.TryGetValue(hubConnectId, out var observer))
            {
                return Task.FromResult<ObservadorAutomacao?>(observer);
            }
            return Task.FromResult<ObservadorAutomacao?>(null);
        }


        public async ValueTask DisposeAsync()
        {
            _descartado = true;

            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }

            foreach (var subscription in _registeredKeys.Values)
            {
                try { subscription.Dispose(); }
                catch { }
            }
            _registeredKeys.Clear();
            _fabricasEscuta.Clear();
            _observersPorChave.Clear();
            _progressoAtualPorHub.Clear();
            _flagCompletouPorHub.Clear();
            _mutexConexao.Dispose();
        }

        private sealed class RepetirSempreRetryPolicy : IRetryPolicy
        {
            public TimeSpan? NextRetryDelay(RetryContext retryContext)
                => retryContext.PreviousRetryCount switch
                {
                    0 => TimeSpan.Zero,
                    1 => TimeSpan.FromSeconds(2),
                    2 => TimeSpan.FromSeconds(5),
                    3 => TimeSpan.FromSeconds(10),
                    _ => TimeSpan.FromSeconds(30)
                };
        }
    }
}
