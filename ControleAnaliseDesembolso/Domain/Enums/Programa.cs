using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ControleAnaliseDesembolso.Domain.Enums
{
    public enum Programa
    {
        [Display(Name = "Pró-Transporte")]
        Pro_Transporte,
        [Display(Name = "Pró-Moradia")]
        Pro_Moradia,
        [Display(Name = "Saneamento Para Todos")]
        Saneamento,
        [Display(Name = "FGTS-Saúde")]
        Saude
    }

    public static class ProgramaExtensions
    {
        public static string ParaExibicao(this Programa programa)
        {
            var campo = typeof(Programa).GetField(programa.ToString());
            var display = campo?.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? programa.ToString();
        }
    }
}
