using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Application.Dtos.Response
{
    public class ComentarioValidacaoResponse
    {
        public int CoMensagem { get; set; }
        public int CoValidacao { get; set; }
        public string? Texto { get; set; }
        public TipoMensagem TipoMensagem { get; set; }
        public string? MatriculaAutor { get; set; }
        public string? NomeAutor { get; set; }
        public int UnidadeAutor { get; set; }
        public string Sigla { get; set; } = string.Empty;
        public DateTime DtCriacao { get; set; }
    }
}
