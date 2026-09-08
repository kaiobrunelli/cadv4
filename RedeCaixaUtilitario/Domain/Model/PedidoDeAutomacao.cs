using System.ComponentModel.DataAnnotations.Schema;

namespace RedeCaixaUtilitario.Domain.Model
{
    public class PedidoDeAutomacao
    {
        public int Id { get; set; }
        public string Servico { get; set; } = "";
        public required string Area { get; set; }
        public required string MatriculaSolicitante { get; set; }
        public DateTime DtSolicitacao { get; set; }
        public DateTime? DtConclusao { get; set; }
        public int Status { get; set; } = 0;
        public int QtdeProcessos { get; set; } = 0;
        public int QtdeProcessosFinalizados { get; set; } = 0;
        public int QtdeFalhas { get; set; } = 0;
        public string Obs { get; set; } = string.Empty;


        [NotMapped]
        public Usuario? Usuario { get; set; }
        [NotMapped]
        public bool MyExtra { get; set; } = false;

    }
}
