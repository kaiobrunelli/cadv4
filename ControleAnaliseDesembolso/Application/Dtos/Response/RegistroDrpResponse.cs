namespace ControleAnaliseDesembolso.Application.Dtos.Response
{
    public class RegistroDrpResponse
    {
        public int Id { get; set; }
        public string Gigov { get; set; } = string.Empty;
        public string ContratoDv { get; set; } = string.Empty;
        public string TipoDesembolso { get; set; } = string.Empty;
        public decimal ValorFgts { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public string ResponsavelBaixa { get; set; } = string.Empty;
        public string Gestor { get; set; } = string.Empty;
        public string? ResponsavelDesembolso { get; set; }

        public int Status { get; set; }
    }
}
