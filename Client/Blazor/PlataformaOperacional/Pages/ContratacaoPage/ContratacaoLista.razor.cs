using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PlataformaOperacional.Components.BlazorComponentes.Dialog;
using PlataformaOperacional.Model.AplicacaoModel;
using PlataformaOperacional.Model.ContratacaoModel;
using PlataformaOperacional.Model.ContratatacaoModel;
using PlataformaOperacional.Service.Middleware;

namespace PlataformaOperacional.Pages.ContratacaoPage
{
    partial class ContratacaoLista
    {
        [Parameter] public string CodigoTomador { get; set; } = "";
        private bool CarregandoListaTabela = false;
        private ContratacaoContrato contratoObjeto = new ();
        private List<ContratacaoContrato> ListaContratos = [];
        private ContratacaoTomador Tomador = new();
        private string FiltroBuscarPalavra = "";
        private ContratacaoContrato selectedItem1 = null;

        protected override async Task OnInitializedAsync()
        {

            Tomador = await HttpResponseHandler.ResponseHandler<ContratacaoTomador>(
                        await ContratacaoServiceInject.ConsultarContratosPorTomador(CodigoTomador), Snackbar) ?? 
                            throw new Exception("Falha ao localizar tomador");

            ListaContratos = Tomador.ListaContratos;
            CarregandoListaTabela = true;
            await InvokeAsync(StateHasChanged);
        }

	

		private async Task DialogoFinalizarContrato(string numeroContrato)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialogInstance = await DialogServiceInject.ShowAsync<DialogConfirmarAcao>("Confirmar finalização de contrato?", options);
            var dialogResult = await dialogInstance.Result;
            if (!dialogResult!.Canceled)
            {
                await FinalizarContrato(numeroContrato);
            }
        }

        public async Task FinalizarContrato(string numeroContrato)
		{
            var response = await ContratacaoServiceInject.FinalizarContrato(numeroContrato);
			await HttpResponseHandler.ResponseHandler(response, Snackbar);
            await OnInitializedAsync();
		}
        public void ConsultarContratoTomadorParaAtualizar(string numeroDoContrato)
        {
            Navigation.NavigateTo($"{Navigation.BaseUri}contratacaoatualizarcontrato/{numeroDoContrato}");
        }

        private IEnumerable<ContratacaoContrato> Elements = new List<ContratacaoContrato>();

        private bool FilterFunc1(ContratacaoContrato element) => FilterFunc(element, FiltroBuscarPalavra);

        private bool FilterFunc(ContratacaoContrato element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Situacao!.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Contrato.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.CoTomador}".Contains(searchString))
                return true;
            return false;
        }
        private async Task VoltarPagina()
        {
            await JsRuntime.InvokeVoidAsync("history.back");
        }
    }
}
