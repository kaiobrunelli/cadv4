using PlataformaOperacional.Model.ContratatacaoModel;
using System.Text.Json.Serialization;


namespace PlataformaOperacional.Model.ContratacaoModel
{
    public class ContratacaoTipoDeVerificacao 
    {            

        [JsonPropertyName("deTipoVerificacao")]
        public string DeTipoVerificacao { get; set; } = "";
        [JsonPropertyName("total")]
        public int Total { get; set; } = 0;
        [JsonPropertyName("concluidos")]
        public int Concluidos { get; set; } = 0;
        [JsonPropertyName("verificacoes")]
        public List<ContratacaoVerificacao> ListaVerificacoes { get; set; } = new List<ContratacaoVerificacao>();
      
    }
}
