using PlataformaOperacional.Model.ContratacaoModel;
using System.Text.Json.Serialization;

namespace PlataformaOperacional.Model.ContratatacaoModel
{
    public class ContratoMock
    {        

        [JsonPropertyName("contrato")]
        public string Contrato { get; set; } = ""; 
        [JsonPropertyName("dtSolicitacao")]
        public DateTime DtSolicitacao { get; set; }
        [JsonPropertyName("dtFinalizacao")]
        public DateTime? DtFinalizacao { get; set; }
        [JsonPropertyName("coSituacao")]
        public int CoSituacao { get; set; }
        [JsonPropertyName("totalVerificacoes")]
        public int TotalVerificacoes;         
        [JsonPropertyName("verificacoesConcluidas")]
        public int VerificacoesConcluidas;
        [JsonPropertyName("tiposVerificacao")]
        public List<ContratacaoTipoDeVerificacao> ListaDeTipoDeVerificacoes { get; set; } = new List<ContratacaoTipoDeVerificacao>();
    }
}
