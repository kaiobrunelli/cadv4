using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PlataformaOperacional.Model.Cobranca.Amortizacao
{
    public abstract class Movimentacao
    {

        [JsonPropertyName("coApontamento")]
        public int? CoApontamento { get; set; }

        [JsonPropertyName("nuContrato")]
        public string? NuContrato { get; set; }

        [JsonPropertyName("nuContratoDv")]
        public string? NuContratoDv { get; set; }

        [JsonPropertyName("tipoMovimentacao")]
        public string? TipoMovimentacao { get; set; }

        [JsonPropertyName("dtCredito")]
        public DateTime? DtCredito { get; set; }

        [JsonPropertyName("vrTotal")]
        public decimal? VrTotal { get; set; }

        [JsonPropertyName("coGifug")]
        public string? CoGifug { get; set; }

        [JsonPropertyName("logInclusaoSiapf")]
        public string? LogInclusaoSiapf { get; set; }

        [JsonPropertyName("saldoDevedorConsiderado")]
        public decimal? SaldoDevedorConsiderado { get; set; }

        [JsonPropertyName("logAcaoUsuario")]
        public string? LogAcaoUsuario { get; set; }

		[JsonPropertyName("logApontamentos")]
		public string? LogApontamentos { get; set; }











		[JsonPropertyName("vrTo01")]
       public decimal? VrTo01 { get; set; }

       [JsonPropertyName("vrTo02")]
       public decimal? VrTo02 { get; set; }

       [JsonPropertyName("vrTo03")]
       public decimal? VrTo03 { get; set; }

       [JsonPropertyName("vrTo04")]
       public decimal? VrTo04 { get; set; }

       [JsonPropertyName("vrTo05")]
       public decimal? VrTo05 { get; set; }

       [JsonPropertyName("vrTo06")]
       public decimal? VrTo06 { get; set; }

       [JsonPropertyName("vrTo07")]
       public decimal? VrTo07 { get; set; }

  











        [NotMapped]
        public bool EmTratamento { get; set; } = false;

        [NotMapped]
        public bool ExpandirObservacao { get; set; } = false;
        [NotMapped]
        public ValoresCCIEditar ValoresCCIEditar { get; set; } = new();



    }
}

