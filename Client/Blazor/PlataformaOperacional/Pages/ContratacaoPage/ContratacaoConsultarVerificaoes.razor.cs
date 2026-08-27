using Microsoft.AspNetCore.Components;
using MudBlazor;
using PlataformaOperacional.Model.ContratatacaoModel;
using PlataformaOperacional.Service.Middleware;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PlataformaOperacional.Pages.ContratacaoPage
{
    partial class ContratacaoConsultarVerificaoes
    {
        [Parameter]
        public string tipoSituacao { get; set; } = "";
        private string rowsPerPageString = "Linhas por página:";
        private string infoFormat = "{first_item}-{last_item} de {all_items}";
        private string FiltroBuscarPalavra = "";
        private string ValueFinalizado = "Finalizado";
        private string ValuePendente = "Com pendências";
        private DateTime? DataInicial { get; set; } = DateTime.Now.Date;
        private DateTime? DataFinal { get; set; } = DateTime.Now.Date;
        public string FiltroRota = "";
        private ContratacaoConsultarResumoTodosContratos selectedItem1 = null;
        public List<ContratacaoConsultarResumoTodosContratos> ListaResumoTodosContratos = new();
        public List<ContratacaoConsultarResumoTodosContratos> datasFiltradas = new();

        public ContratacaoConsultarResumoGeralChecklist ConsultaGeralCheckList = new();
        public List<ContratacaoConsultarResumoGeralChecklist> ListaGeralCheckList = new();

        public int contagemCOntratos;

        protected override async Task OnInitializedAsync()
        {

            ListaGeralCheckList = await HttpResponseHandler.ResponseHandler<List<ContratacaoConsultarResumoGeralChecklist>>(
                        await ContratacaoServiceInject.ConsultarResumoGeralChecklist(), Snackbar) ??
                            throw new Exception("Falha ao consultar resumo geral.");

            ConsultaGeralCheckList = ListaGeralCheckList.First();

            ListaResumoTodosContratos = await HttpResponseHandler.ResponseHandler<List<ContratacaoConsultarResumoTodosContratos>>(
                        await ContratacaoServiceInject.ConsultarResumoTodosContratos(), Snackbar) ??
                            throw new Exception("Falha ao resumo de todos os contratos.");

            datasFiltradas = ListaResumoTodosContratos;

            var uri = new Uri(Navigation.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            string filtroSelecionado = query["filtro"] ?? "";
            FilterByCard(filtroSelecionado);
            await InvokeAsync(StateHasChanged);
        }
        public void ConsultarContratoTomadorParaAtualizar(string numeroDoContrato)
        {      
        
            Navigation.NavigateTo($"{Navigation.BaseUri}contratacaoatualizarcontrato/{numeroDoContrato}");        
        }

        private IEnumerable<ContratacaoConsultarResumoTodosContratos> Elements = new List<ContratacaoConsultarResumoTodosContratos>();

        private bool FilterFunc1(ContratacaoConsultarResumoTodosContratos element) => FilterFunc(element, FiltroBuscarPalavra);

        private bool FilterFunc(ContratacaoConsultarResumoTodosContratos element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Situacao.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Contrato.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.CoTomador}".Contains(searchString))
                return true; 
            return false;
        }
      
        private void FilterByCard(string elementName)
        {
            FiltroBuscarPalavra = elementName;
        }

        private void ClearFilter()
        {
            FiltroBuscarPalavra = "";
            datasFiltradas = ListaResumoTodosContratos;
        }

        private void FiltraData()
        {
            if (DataInicial.HasValue && DataFinal.HasValue)
            {
                if (DataFinal < DataInicial)
                {
                    Snackbar.Add("Data final deve ser maior ou igual à data inicial.", Severity.Info);
                    return;
                }

                datasFiltradas = ListaResumoTodosContratos.Where(d => d.DataSolicitacao >= DataInicial.Value && d.DataSolicitacao <= DataFinal.Value).ToList();
            }
            else
            {
                datasFiltradas = ListaResumoTodosContratos;
            }

            if (ListaResumoTodosContratos.Count == 0)
            {
                Snackbar.Add("Não há contratos nesse período.", Severity.Info);
            }
        }      
    }
}

