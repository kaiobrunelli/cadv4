using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PlataformaOperacional.Components.BlazorComponentes.Dialog;
using PlataformaOperacional.Model.AplicacaoModel;
using PlataformaOperacional.Model.ContratacaoModel;
using PlataformaOperacional.Model.ContratatacaoModel;
using PlataformaOperacional.Service.Middleware;
using System.Net.Http.Json;
using System.Text.Json;

namespace PlataformaOperacional.Pages.ContratacaoPage;


partial class ContratacaoAtualizarContrato
{
    public bool Basic_CheckBox2;

    [Parameter]
    public string CodigoTomador { get; set; } = "";

    [Parameter]
    public string NumeroDoContrato { get; set; } = "";

	public string respostaRessalva { get; set; } = "";
    public bool CarregandoListaTabela = false;
    private string UsuarioConclusaoRessalva = "";
    public ContratacaoContrato ContratoPreencher { get; set; } = new();

    public List<ContratacaoVerificacao> ListaVerificacoes = [];

    public List<ContratacaoRessalva> ListaDeRessalvasPorContrato = [];

	protected override async Task OnInitializedAsync()
	{
		var responseContratoId = await ContratacaoServiceInject.ConsultarContratoPorId(NumeroDoContrato);
		ContratoPreencher = await HttpResponseHandler.ResponseHandler<ContratacaoContrato>(responseContratoId, Snackbar);

		var resalvaPorContrato = await ContratacaoServiceInject.ConsultarRessalvaPorContrato(NumeroDoContrato);
		ListaDeRessalvasPorContrato = await HttpResponseHandler.ResponseHandler<List<ContratacaoRessalva>>(resalvaPorContrato, Snackbar);
               
        CarregandoListaTabela = true;

		StateHasChanged();
	}

	private async Task DialogoFinalizarContrato(string numeroContrato)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var dialogInstance = await DialogServiceInject.ShowAsync<DialogConfirmarAcao>("Confirmar finalização de contrato?", options);
        var dialogResult = await dialogInstance.Result;
        if (!dialogResult!.Canceled)
        {
            await FinalizarContrato(numeroContrato);
            await OnInitializedAsync();
        }
    }
    public async Task FinalizarContrato(string numeroContrato)
    {
        var response = await ContratacaoServiceInject.FinalizarContrato(numeroContrato);
		await HttpResponseHandler.ResponseHandler(response, Snackbar);
    }

	private async Task DialogoCancelarFinalizacaoContrato(string numeroContrato)
	{
		var options = new DialogOptions { CloseOnEscapeKey = true };
		var dialogInstance = await DialogServiceInject.ShowAsync<DialogConfirmarReversao>("Reverter a finalização do contrato?", options);
		var dialogResult = await dialogInstance.Result;
		if (!dialogResult!.Canceled)
		{
			await CancelarFinalizacaoContrato(numeroContrato);
			await OnInitializedAsync();
		}
	}

	public async Task CancelarFinalizacaoContrato(string numeroContrato)
	{
		var response = await ContratacaoServiceInject.CancelarFinalizacaoContrato(numeroContrato);
		await HttpResponseHandler.ResponseHandler(response, Snackbar);
	}

    private async Task DialogFinalizarRessalva(ContratacaoRessalva contratacaoRessalva, bool value)
    {
        if (value)
        {
            var options = new DialogOptions { CloseButton = true, FullWidth = true, BackdropClick = false };

            var dialogReference = await DialogServiceInject.ShowAsync<DialogFinalizarRessalva>("Finalizar ressalva", options);
            var result = await dialogReference.Result;

            if (!result.Canceled)
            {
                var observacaoPreenchida = result.Data as string;

                await FinalizarRessalva(contratacaoRessalva, value, observacaoPreenchida);
                await InvokeAsync(StateHasChanged);
            }
        }

        else 
        {
            var options = new DialogOptions { CloseButton = true, FullWidth = true, BackdropClick = false };

            var dialogReference = await DialogServiceInject.ShowAsync<DialogDesmarcarTratamentoRessalva>("Cancelar tratamento", options);
            var result = await dialogReference.Result;

            if (!result.Canceled)
            {
                await FinalizarRessalva(contratacaoRessalva, value, string.Empty);
                await InvokeAsync(StateHasChanged);
            }
        }
    }
    public async Task FinalizarRessalva(ContratacaoRessalva contratacaoRessalva, bool value, string observacaoPreenchida)
    {
        contratacaoRessalva.Tratado = value;

		if (contratacaoRessalva.IdChecklist.HasValue)
        {
            if (value)
            {
				await HttpResponseHandler.ResponseHandler(
                    await ContratacaoServiceInject.FinalizarRessalva(contratacaoRessalva.Id, value, observacaoPreenchida), Snackbar);

				await HttpResponseHandler.ResponseHandler(
                    await ContratacaoServiceInject.AtualizarCheckList(contratacaoRessalva.IdChecklist.Value, 1), Snackbar);
			}
            else
            {
				
				await HttpResponseHandler.ResponseHandler(
                    await ContratacaoServiceInject.AtualizarCheckList(contratacaoRessalva.IdChecklist.Value, 2), Snackbar);

				await HttpResponseHandler.ResponseHandler(
                    await ContratacaoServiceInject.FinalizarRessalva(contratacaoRessalva.Id, value, observacaoPreenchida), Snackbar);
			}
                 
        }
        else
        {
			await HttpResponseHandler.ResponseHandler(
					await ContratacaoServiceInject.FinalizarRessalva(contratacaoRessalva.Id, value, observacaoPreenchida), Snackbar);
		}

		var resalvaPorContrato = await ContratacaoServiceInject.ConsultarRessalvaPorContrato(NumeroDoContrato);
		ListaDeRessalvasPorContrato = await HttpResponseHandler.ResponseHandler<List<ContratacaoRessalva>>(resalvaPorContrato, Snackbar);

		var responseContratoId = await ContratacaoServiceInject.ConsultarContratoPorId(NumeroDoContrato);
		ContratoPreencher = await HttpResponseHandler.ResponseHandler<ContratacaoContrato>(responseContratoId, Snackbar);

	}

	[Inject] private IDialogService DialogService { get; set; }
	public async void consultarObservacaoRessalva(string observacao) 
    {
        await DialogService.ShowMessageBoxAsync("Observação tratamento", observacao, yesText:"Ok");
    }
    
    public async Task AtualizarCheckListContrato(int id, int? resposta)
    {       
        var response = await ContratacaoServiceInject.AtualizarCheckList(id, resposta);
		await HttpResponseHandler.ResponseHandler(response, Snackbar);
        await OnInitializedAsync();
        await InvokeAsync(StateHasChanged);
    }

    public async Task DialogRessalva(int? value, ContratacaoVerificacao itemVerificacao)
    {
        UsuarioConclusaoRessalva = itemVerificacao.Usuario;

		if (value == 2 && itemVerificacao.TemObs == true)
        {
            var options = new DialogOptions { CloseButton = true, FullWidth = true, BackdropClick = false };
            var dialogReference = await DialogServiceInject.ShowAsync<DialogContratacaoAdicionarRessalva>("Ressalva", options);
            var result = await dialogReference.Result;

            if (!result!.Canceled)
            {
                itemVerificacao.Resposta = value;
                var observacaoPreenchida = result.Data as string;           

                var response = await ContratacaoServiceInject.AtualizarCheckList(itemVerificacao.IdChecklist, value);
				await HttpResponseHandler.ResponseHandler(response, Snackbar);
				await AdicionarRessalvaCheckList(itemVerificacao.IdChecklist, itemVerificacao.IdVerificacao, observacaoPreenchida!, NumeroDoContrato, 0);
				await HttpResponseHandler.ResponseHandler(response, Snackbar);
            
                await OnInitializedAsync();

            }
            else
            {           
                if (itemVerificacao.Resposta == 1 || itemVerificacao.Resposta == 3) return;
                itemVerificacao.Resposta = null;    
				await OnInitializedAsync();         
            }
        }
        else
        {
            itemVerificacao.Resposta = value;
            var response = await ContratacaoServiceInject.AtualizarCheckList(itemVerificacao.IdChecklist, value);
			await HttpResponseHandler.ResponseHandler(response, Snackbar);
			await OnInitializedAsync();
   
        }

    }


    public async Task AdicionarRessalvaCheckList(int? idChecklist, int? idChecklistVerificacao, string observacao, string contrato, int? idCategoriaObs)
    {
        var response = await ContratacaoServiceInject.CadastrarRessalva(idChecklist, idChecklistVerificacao, observacao, contrato, idCategoriaObs);
		await HttpResponseHandler.ResponseHandler(response, Snackbar);

		var responseContratoId = await ContratacaoServiceInject.ConsultarContratoPorId(NumeroDoContrato);
		ContratoPreencher = await HttpResponseHandler.ResponseHandler<ContratacaoContrato>(responseContratoId, Snackbar);

        await OnInitializedAsync();
        await InvokeAsync(StateHasChanged);
    }
    void ChangePosition(string message, string position)
    {
        Snackbar.Clear();
        Snackbar.Configuration.PositionClass = position;
        Snackbar.Add(message, Severity.Normal);
    }

    private async Task DialogAdicionarRessalva()
    {
        var options = new DialogOptions { CloseButton = true, FullWidth = true, BackdropClick = false };

        var dialogReference = await DialogServiceInject.ShowAsync<DialogContratacaoAdicionarRessalva>("Ressalva", options);
        var result = await dialogReference.Result;

        if (!result.Canceled)
        {
            var observacaoPreenchida = result.Data as string;

            await AdicionarRessalvaCheckList(null, null, observacaoPreenchida!, NumeroDoContrato, 0);
            await InvokeAsync(StateHasChanged);
        }
    }


    private async Task DialogDesativarRessalva(ContratacaoRessalva ressalva)
    {
        var options = new DialogOptions { CloseButton = true, FullWidth = true, BackdropClick = false };
        var dialogReference = await DialogServiceInject.ShowAsync<DialogDesativarRessalva>("Excluir ressalva (Homologação)", options);
        var result = await dialogReference.Result;

        if (!result.Canceled)
        {
            await DesativarRessalva(ressalva.Id);
			if (ressalva.IdChecklist != null)
			{
				await ContratacaoServiceInject.AtualizarCheckList(ressalva.IdChecklist, null);
			}
			var responseContratoId = await ContratacaoServiceInject.ConsultarContratoPorId(NumeroDoContrato);
			ContratoPreencher = await HttpResponseHandler.ResponseHandler<ContratacaoContrato>(responseContratoId, Snackbar);

		}
    }

    public async Task DesativarRessalva(int idRessalva)
    {
        var response = await ContratacaoServiceInject.DesativarRessalva(idRessalva);
		await HttpResponseHandler.ResponseHandler(response, Snackbar);
        await OnInitializedAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetProposta(ContratacaoVerificacao itemVerificacao)
    {
        if (itemVerificacao.Resposta is null) return;

		var response = await ContratacaoServiceInject.AtualizarCheckList(itemVerificacao.IdChecklist, itemVerificacao.Resposta);
		await HttpResponseHandler.ResponseHandler(response, Snackbar);

        if (response.IsSuccessStatusCode)
        {
            itemVerificacao.Resposta = null;
            itemVerificacao.Usuario = null;
        }
		await InvokeAsync(StateHasChanged);
    }

    private async Task VoltarPagina()
    {
        await JsRuntime.InvokeVoidAsync("history.back");
    }
}

