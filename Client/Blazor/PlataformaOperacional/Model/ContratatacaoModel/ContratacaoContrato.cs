using Microsoft.Extensions.Primitives;
using System.Text.Json.Serialization;

namespace PlataformaOperacional.Model.ContratacaoModel
{
    public class ContratacaoContrato
    {     

        [JsonPropertyName("contrato")]
        public string Contrato { get; set; } = "";
        [JsonPropertyName("coTomador")]
        public string CoTomador { get; set; } = "";
        [JsonPropertyName("dtAssinatura")]
        public DateTime DataAssinatura { get; set; }
        [JsonPropertyName("dtSolicitacao")]
        public DateTime DtSolicitacao { get; set; }
        [JsonPropertyName("dtFinalizacao")]
        public DateTime? DtFinalizacao { get; set; }
        [JsonPropertyName("situacao")]
        public string? Situacao { get; set; }
		[JsonPropertyName("coSituacao")]
		public int? CoSituacao { get; set; }
		[JsonPropertyName("totalVerificacoes")]
        public int? TotalVerificacoes { get; set; }
        [JsonPropertyName("concluidos")]
        public int? Concluidos { get; set; } 
        [JsonPropertyName("pendentes")]
        public int? Pendentes { get; set; } = 0;
        [JsonPropertyName("irregular")]
        public int? Irregular { get; set; }
        [JsonPropertyName("verificacoesConcluidas")]
        public int? VerificacoesConcluidas { get; set; }
        [JsonPropertyName("tiposVerificacao")]      
        public List<ContratacaoTipoDeVerificacao> ListaDeTipoDeVerificacoes { get; set; } = new List<ContratacaoTipoDeVerificacao>();
       
        [JsonPropertyName("podeFinalizar")]            
        public bool PodeFinalizar { get; set; }
        [JsonPropertyName("podeReverterFinalizacao")]
        public bool podeReverterFinalizacao { get; set; }
    }
}

