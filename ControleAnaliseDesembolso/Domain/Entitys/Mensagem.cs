using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Domain.Entitys
{
    public class Mensagem
    {
        public int CoMensagem { get; set; }
        public int CoValidacao { get; set; }
        public int CoControleDesembolso { get; set; }
        public string? DeMensagem { get; set; }
        public TipoMensagem TipoMensagem { get; set; }
        public string? CoUsuario { get; set; }
        public string? DeUsuario { get; set; }
        public int UnidadeUsuario { get; set; }
        public DateTime DtCriacao { get; set; } = DateTime.Now;

        public bool Ativo { get; set; } = true;

        public string SiglaUsuario => UnidadeUsuario == 7175 ? "CEFGA" : "GIGOV";
    }
}
