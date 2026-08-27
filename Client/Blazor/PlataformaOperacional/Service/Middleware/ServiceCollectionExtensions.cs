using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PlataformaOperacional.Model.Plataforma;

public static class ServiceCollectionExtensions
{



	public static IServiceCollection AddProjectHttpClientsPlataforma(this IServiceCollection services, IWebAssemblyHostEnvironment hostEnvironment)
	{
	
		var isDev = hostEnvironment.IsDevelopment();


		var baseAddressPrincipal = isDev
			? "http://localhost:5079/"
			: "https://www.ativo.fgts.caixa/PlataformaOperacional/";


		var baseAddressLocalWwwRoot = hostEnvironment.BaseAddress;

	
		services.AddHttpClient("Api", client => {
			client.BaseAddress = new Uri(baseAddressPrincipal);		
		});

		services.AddHttpClient("ApiLocal", client => {
			client.BaseAddress = new Uri(baseAddressLocalWwwRoot);
		});

		return services;
	}

	
}
