using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Application.Dtos.Response
{
    public class DesembolsoResponse
    {
        public int CoControleDesembolso { get; set; }
        public string Id { get; set; } = string.Empty;
        public string NumId { get; set; } = string.Empty;
        public DateTime DtSolicitado { get; set; }

        public string Contrato { get; set; } = string.Empty;
        public string Mutuario { get; set; } = string.Empty;
        public string Gigov { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string AgenteFinanceiro { get; set; } = string.Empty;
        public string AgentePromotor { get; set; } = string.Empty;
        public string MatriculaSolicitante { get; set; } = string.Empty;

        public int ValidacoesOk { get; set; }
        public int ValidacoesTotal { get; set; }
        public TipoStatusDesembolso Status { get; set; }

        public bool PrimeiroDesembolso { get; set; }
        public bool Adiantamento { get; set; }
        public bool UltimoDesembolso { get; set; }
        public bool Recorrente { get; set; }

        public DateTime PrazoFinal { get; set; }
        public bool DataAgendamento { get; set; } = false;
        public bool Sanepar { get; set; }
        public string? ContratoAo { get; set; }
        public string? ContratoAoDv { get; set; }
        public string? ResponsavelAnalise { get; set; }
        public DateTime? DtConclusao { get; set; }
    }
}
