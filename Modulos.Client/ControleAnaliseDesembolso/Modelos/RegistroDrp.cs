namespace ControleAnaliseDesembolso.Modelos;

public class RegistroDrp
{
    public int      Id            { get; set; }
    public string   Gigov         { get; set; } = "";
    public string   ContratoDv    { get; set; } = "";
    public string   TipoDesembolso{ get; set; } = "";
    public decimal  ValorFgts     { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public string   ResponsavelBaixa { get; set; } = "";
    public string   Gestor        { get; set; } = "";
    public string?  ResponsavelDesembolso { get; set; }
    public int      Status        { get; set; }

    public bool Baixado    => Status == 5;
    public bool Rejeitado  => Status == 4;
    public bool Cancelado  => Status == 6;
}
