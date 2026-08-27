using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PlataformaOperacional.Model.Cobranca.Amortizacao
{
	public class MovimentacaoDrp : Movimentacao
	{
      



        [JsonPropertyName("idDrp")]
        public int IdDrp { get; set; }

        [JsonPropertyName("gifug")]
        public string? Gifug { get; set; }

        [JsonPropertyName("gifugDv")]
        public string? GifugDv { get; set; }

        [JsonPropertyName("tomador")]
        public string? Tomador { get; set; }

        [JsonPropertyName("nuDrp")]
        public string? NuDrp { get; set; }

        [JsonPropertyName("dvDrp")]
        public string? DvDrp { get; set; }

        [JsonPropertyName("senha")]
        public string? Senha { get; set; }

        [JsonPropertyName("qtdMovimentacoes")]
        public int? QtdMovimentacoes { get; set; }

        [JsonPropertyName("valor")]
        public decimal? Valor { get; set; }

        [JsonPropertyName("deObservacoes")]
        public string? DeObservacoes { get; set; }

   


























    }

}
