using ControleAnaliseDesembolso.Application;
using ControleAnaliseDesembolso.Application.Interface;
using ControleAnaliseDesembolso.Domain.Repositorys;
using ControleAnaliseDesembolso.Hubs;
using ControleAnaliseDesembolso.Infra.Datas.Context;
using ControleAnaliseDesembolso.Infra.Datas.Repositorys;
using ControleAnaliseDesembolso.Interface;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlataformaNotificacao.Application;
using PlataformaNotificacao.Application.Interface;
using PlataformaNotificacao.Infra.Context;
using PlataformaOperacional.Application.Service;
using PlataformaOperacional.Application.Service.Interface;
using RedeCaixaUtilitario.Application;
using RedeCaixaUtilitario.Application.Interface;
using Utilitarios.Service;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ControleAnaliseDesembolso")
    ?? throw new InvalidOperationException("Connection string 'ControleAnaliseDesembolso' não configurada.");

var connectionStringNotificacao = builder.Configuration.GetConnectionString("PlataformaNotificacao")
    ?? throw new InvalidOperationException("Connection string 'PlataformaNotificacao' não configurada.");

builder.Services.AddDbContext<ControleAnaliseDesembolsoContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IControleAnaliseDesembolsoService, ControleAnaliseDesembolsoService>();
builder.Services.AddScoped<IFichaPedidoDesembolsoService, FichaPedidoDesembolsoService>();
builder.Services.AddScoped<IValidadorDesembolsoService, ValidadorDesembolsoService>();
builder.Services.AddScoped<IEmpregadoCADService, EmpregadoCADService>();
builder.Services.AddScoped<UtilitarioMapperServicecopy>();

// Repositório vindo de XP Metodo nvoo/ControleAnaliseDesembolsoRepositorio —
// mesmos nomes de classe/método do projeto de referência. Cobre só as
// consultas/gravações que já existiam lá; tudo que foi construído depois
// nesse CAD (conferência de campos, SIAPF, cancelamento, OBS CEFGA...)
// continua acessando o DbContext direto dentro do service, como já era.
builder.Services.AddScoped<IRepositorioDesembolso, RepositorioDesembolso>();
builder.Services.AddScoped<IRepositorioControle, RepositorioControle>();
builder.Services.AddScoped<IRepositorioMensagem, RepositorioMensagem>();
builder.Services.AddScoped<IRepositorioValidacao, RepositorioValidacao>();
builder.Services.AddScoped<IRepositorioValidacaoControle, RepositorioValidacaoControle>();

// SiapfService real — automação de terminal 3270 (ConsultarCadastroGeral),
// usada em ControleAnaliseDesembolsoService.ExecutarConferenciaCamposInterno
// pra confrontar o FPD com o cadastro real do contrato no SIAPF.
//
// >>> MODO TESTE (temporário) <<<
// Trocado pro SiapfServiceMock só pra testar a tela de conferência sem
// terminal 3270 de verdade. COMO VOLTAR AO NORMAL: comente a linha do Mock
// abaixo e descomente a linha do SiapfService real (ou apague
// RedeCaixaUtilitario/Application/SiapfServiceMock.cs, dá no mesmo).
// builder.Services.AddScoped<ISiapfService, SiapfService>();
builder.Services.AddScoped<ISiapfService, SiapfServiceMock>();

builder.Services.AddScoped<IAplicacaoService, AplicacaoService>();

builder.Services.AddScoped<INotificacaoService>(provider =>
{
    var servico = new NotificacaoService(connectionStringNotificacao);
    var signalR = provider.GetRequiredService<SignalRNotificacaoService>();
    servico.OnNotificacao += (sender, e) => _ = signalR.HandlerObserver(sender, e);
    return servico;
});

builder.Services.AddScoped<PlataformaNotificacaoContext>(_ => new PlataformaNotificacaoContext(connectionStringNotificacao));
builder.Services.AddScoped<IEmpregadoService, EmpregadoService>();

builder.Services.AddSignalR();
builder.Services.AddScoped<SignalRNotificacaoService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string CorsWasmDev = "CorsWasmDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsWasmDev, policy => policy
        .WithOrigins("http://localhost:5181", "https://localhost:7224")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var erro = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro ao processar a solicitação",
            Detail = erro?.Message,
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsWasmDev);

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
