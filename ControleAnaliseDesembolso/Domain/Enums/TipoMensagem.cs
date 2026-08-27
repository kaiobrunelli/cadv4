using System.Text.Json.Serialization;

namespace ControleAnaliseDesembolso.Domain.Enums
{
    [JsonConverter(typeof(CamelCaseStringEnumConverter))]
    public enum TipoMensagem
    {
        JUSTIFICATIVA,
        INFORMATIVO,
        PARECER,
        OBSERVACAO,
        REJEICAO
    }
}
