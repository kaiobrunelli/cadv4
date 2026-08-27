using PlataformaOperacional.Model.Contabilidade;

namespace PlataformaOperacional.Pages.Contabilidade
{
    public partial class ContabilidadeConsultar
    {
        public string Nova_Propriedade_Testar_GIT {  get; set; } = "";

        private string searchString1 = "";

        public List<ConsultaLotes> ListaContabilidadeConsulta { get; set; } = new List<ConsultaLotes>();
        public ConsultaLotes? ContabilidadeConsultaPorNumero { get; set; }
        private DateTime? DataInicial { get; set; } = DateTime.Now.Date;
        private DateTime? DataFinal { get; set; } = DateTime.Now.Date;
        private string NumeroLote { get; set; } = "";
        private string NumeroMatricula { get; set; } = "";  
        private string ErroMessage { get; set; } = "";
        private bool MostrarTabela = false;
      
        private void HandlerFocus()
        {
            NumeroLote = string.Empty;
            NumeroMatricula = string.Empty;
        }
        private async Task BuscarLoteOuMatricula()
        {
            MostrarTabela = true;
            ListaContabilidadeConsulta.Clear();  
            if (DataInicial!.Value > DataFinal!.Value)
            {
                ErroMessage = $"Data inicial não pode ser maior que a data final!";
                return;
            }

            if (!string.IsNullOrWhiteSpace(NumeroMatricula))
            {                
                await ConsultaLotePorMatricula(NumeroMatricula);
            }
            else if (NumeroLote !=null)
            {
                await ConsultaLotePorNumero(int.Parse(NumeroLote));
            }
            else
            {
                ErroMessage = "Necessário preencher Lote ou Mátrícula.";
            }
        }   

        public async Task BuscarListaLotesContabilidade()
        {
            ListaContabilidadeConsulta = await ContabilidadeService.ControleLotesMovimentacoesContabeis();
            StateHasChanged();
        }
        public async Task ConsultaLotePorNumero(int numeroLoteRequest)
        {            
            ContabilidadeConsultaPorNumero = await ContabilidadeService.ConsultaLotePorNumero(numeroLoteRequest);
            ListaContabilidadeConsulta.Add(ContabilidadeConsultaPorNumero);
            StateHasChanged();                  
        }
        public async Task ConsultaLotePorMatricula(string matriculaRequest)
        {
           
            ListaContabilidadeConsulta = await ContabilidadeService.ConsultaLotePorMatricula(matriculaRequest);
            StateHasChanged();
        }
     
        public async Task ConsultaLoteEmAberto(string matriculaRequest)
        {
            ListaContabilidadeConsulta = await ContabilidadeService.ConsultaLoteEmAberto(matriculaRequest);
            StateHasChanged();
        }
        public async Task<string> CapturarMovimentacoesContabeis(MovimentacaoContabil movimentacaoContabilRequest)
        {
            var response = await ContabilidadeService.CapturaMovimentacoesContabeis(movimentacaoContabilRequest);
            StateHasChanged();
            return response;
        }

        private bool FilterFunc1(ConsultaLotes element) => FilterFunc(element, searchString1);
        private bool FilterFunc(ConsultaLotes element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if ($"{element.Carregado}".Contains(searchString))
                return true;
            return false;
        }
    }
}
