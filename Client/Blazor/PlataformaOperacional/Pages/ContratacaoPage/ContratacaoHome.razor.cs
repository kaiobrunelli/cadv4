


using MudBlazor;
using PlataformaOperacional.Model.ContratatacaoModel;
using PlataformaOperacional.Service.Middleware;



namespace PlataformaOperacional.Pages.ContratacaoPage
{
    partial class ContratacaoHome
    {
        public bool CarregandoHomePage = false;
        public List<ContratacaoTomador> ListaTomadores = new List<ContratacaoTomador>();
        public ContratacaoConsultarResumoGeralChecklist ConsultaGeralCheckList = new();
        public List<ContratacaoConsultarResumoGeralChecklist> ListaGeralCheckList = new();
        private MudTheme _myCustomTheme = new();

        protected override async Task OnInitializedAsync()
        {
            ListaGeralCheckList = await HttpResponseHandler.ResponseHandler<List<ContratacaoConsultarResumoGeralChecklist>>(
                        await ContratacaoServiceInject.ConsultarResumoGeralChecklist(), Snackbar) ??
                            throw new Exception("Falha ao consultar resumo geral.");

            ConsultaGeralCheckList = ListaGeralCheckList.First();

            ListaTomadores = await HttpResponseHandler.ResponseHandler<List<ContratacaoTomador>>(
                        await ContratacaoServiceInject.ConsultarResumoTomadores(), Snackbar) ??
                            throw new Exception("Falha ao consultar resumo geral.");


            CarregandoHomePage = true;
            await InvokeAsync(StateHasChanged);
        }
        public void ConsultarTomador(string codigoTomador)
        {
            Navigation.NavigateTo($"{Navigation.BaseUri}contratacaolistatomador/{codigoTomador}");
        }
        public void ConsultarSituacoes()
        {
            Navigation.NavigateTo($"{Navigation.BaseUri}consultarverificacoes");
        }
        public void ConsultarVerificacoes(string filtro)
        {
            Navigation.NavigateTo($"{Navigation.BaseUri}consultarverificacoes?filtro={filtro}");
        }
    }
}
