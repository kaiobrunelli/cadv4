namespace ControleAnaliseDesembolso.Modelos;

public class DesembolsoCAD
{
    public string Id { get; set; } = "";
    public string NumId { get; set; } = "";
    public string Contrato { get; set; } = "";
    public string Mutuario { get; set; } = "";
    public string Gigov { get; set; } = "";
    public decimal Valor { get; set; }
    public string AgenteFinanceiro { get; set; } = "";
    public string AgentePromotor { get; set; } = "";
    public string MatriculaSolicitante { get; set; } = "";
    public string Fase { get; set; } = "";
    public int ValidacoesOk { get; set; }
    public int ValidacoesTotal { get; set; }
    public string Status { get; set; } = "";
    public DateTime PrazoFinal { get; set; }
    public DateTime DtSolicitado { get; set; }
    public DateTime? DtConclusao { get; set; }
    public bool PrimeiroDesembolso { get; set; }
    public bool UltimoDesembolso { get; set; }
    public bool Adiantamento { get; set; }
    public string? ResponsavelAnalise { get; set; }
}
