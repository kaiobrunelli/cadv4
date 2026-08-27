using ControleAnaliseDesembolso.Service;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using PlataformaNotificacao.UI.Servicos;
using PlataformaOperacional;
using PlataformaOperacional.Model.Plataforma;
using PlataformaOperacional.Service;
using PlataformaOperacional.Service.AplicacaoService;
using PlataformaOperacional.Service.AplicacaoService.Preditor;
using PlataformaOperacional.Service.Cobranca;
using PlataformaOperacional.Service.Cobranca.EncontroDeContas;
using PlataformaOperacional.Service.Contabilidade;
using PlataformaOperacional.Service.ContratacaoService;
using PlataformaOperacional.Service.CRFService;
using PlataformaOperacional.Service.Middleware;
using PlataformaOperacional.Service.Monitoramento.EntradaDeDadosService;
using SipubDesembolsos.Client.Servicos;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Configuration.AddJsonFile("appsettings.json",optional:false,reloadOnChange:true);
builder.Configuration.AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json",optional:true,reloadOnChange:true);
var baseHref = builder.Configuration.GetValue<string>("BasePath");
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.IsDevelopment()
        ? "http://localhost:5079/"
        : builder.HostEnvironment.BaseAddress)
});

string baseAdress = "";




builder.Services.AddProjectHttpClientsPlataforma(builder.HostEnvironment);








builder.Services.Configure<SimulacaoA>(builder.Configuration.GetSection("SimulacaoA"));
builder.Services.Configure<SimulacaoB>(builder.Configuration.GetSection("SimulacaoB"));
builder.Services.AddSingleton<MudThemeService>();
builder.Services.AddSingleton(provider => new BlazorMockService(true));
builder.Services.AddSingleton<SignalRService>();
builder.Services.AddScoped<DialogServicePlataformaOperacional>();
builder.Services.AddScoped<ContabilidadeService>();
builder.Services.AddScoped<PlataformaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ContratacaoService>();   
builder.Services.AddScoped<CRFService>();   
builder.Services.AddScoped<EncryptionService>();   
builder.Services.AddScoped<SaldoResidualService>();
builder.Services.AddScoped<CobrancaService>();
builder.Services.AddScoped<PreditorService>();
builder.Services.AddScoped<DownloadService>();
builder.Services.AddScoped<DesembolsosService>();
builder.Services.AddScoped<EntradaDeDadosService>();
builder.Services.AddScoped<EncontroDeContasService>();
builder.Services.AddScoped<ServicoUsuario>();
builder.Services.AddSingleton<ServicoNotificacao>();
builder.Services.AddSingleton<NotificacaoServiceUI>();
builder.Services.AddScoped<ControleAnaliseDesembolsoService>();




var urlServidor = builder.Configuration["ServidorUrl"] ?? "https://localhost:7211";
builder.Services.AddSingleton<ServicoUsuarioUI>();
builder.Services.AddSingleton<NotificacaoServiceSignalRUI>();
builder.Services.AddSingleton( _ => new SignalRServiceUI(urlServidor));






await builder.Build().RunAsync();


