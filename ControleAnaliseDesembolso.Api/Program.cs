using ControleAnaliseDesembolso.Application;
using ControleAnaliseDesembolso.Application.Interface;
using ControleAnaliseDesembolso.Hubs;
using ControleAnaliseDesembolso.Infra.Datas.Context;
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

// DI simulada do RedeCaixaUtilitario — só o SiapfService (mock), sem terminal 3270 real.
// Ver ControleAnaliseDesembolsoService.ExecutarValidacaoDesembolso pro uso na validação de VALORES.
builder.Services.AddScoped<ISiapfService, SiapfService>();

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
