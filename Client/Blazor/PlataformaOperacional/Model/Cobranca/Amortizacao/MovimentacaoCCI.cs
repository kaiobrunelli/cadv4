using System.Text.Json.Serialization;

namespace PlataformaOperacional.Model.Cobranca.Amortizacao
{
    public class MovimentacaoCCI : Movimentacao
    {
        [JsonPropertyName("coMovimentacao")]
        public int? CoMovimentacao { get; set; } 
        [JsonPropertyName("nuContratoOrigem")]
        public string? NuContratoOrigem { get; set; }

        [JsonPropertyName("nuContratoOrigemDv")]
        public string? NuContratoOrigemDv { get; set; }

    }
}
