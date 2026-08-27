using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Application.Dtos.Request
{
    public class ValidacaoRegistroRequest
    {
        public string? DeMensagem { get; set; }
        public TipoMensagem TipoMensagem { get; set; }
        public string MatriculaAutor { get; set; } = string.Empty;
        public string? NomeAutor { get; set; }
        public int UnidadeAutor { get; set; }
    }
}
