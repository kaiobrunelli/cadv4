using System.Text.Json.Serialization;

namespace PlataformaOperacional.Model.ContratatacaoModel
{
    public class ContrataXP
    {

        [JsonPropertyName("idChecklist")]
        public int IdChecklist { get; set; }
        [JsonPropertyName("deVerificacao")]
        public string? DeVerificacao { get; set; } = "";
        [JsonPropertyName("dtAnalise")]
        public DateTime? DtAnalise { get; set; }
        [JsonPropertyName("resposta")]
        public int Resposta { get; set; }
        [JsonPropertyName("temObs")]
        public bool TemObs { get; set; }
        [JsonPropertyName("observacao")]
        public string? Observacao { get; set; }
    }
}
