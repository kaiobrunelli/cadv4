using ControleAnaliseDesembolso.Domain.Enums;

namespace ControleAnaliseDesembolso.Application.Dtos.Request
{
    public class EditarComentarioRequest
    {
        public int CoMensagem { get; set; }
        public string DeMensagem { get; set; } = string.Empty;
        public string MatriculaSolicitante { get; set; } = string.Empty;

        public TipoMensagem TipoMensagem { get; set; }
    }
}
