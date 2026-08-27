using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace PlataformaOperacional.Pages.Aplicacao.Desembolsos;

public partial class Validador : ComponentBase
{
    public static readonly MudTheme DSCtheme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#005CA9",
            PrimaryDarken = "#00437A",
            PrimaryLighten = "#2D8AD8",
            Secondary = "#d87b00",
            SecondaryDarken = "#a65e00",
            SecondaryLighten = "#f39200",
            Tertiary = "#54bbab",
            TertiaryDarken = "#359485",
            TertiaryLighten = "#81d6c8",
            GrayDefault = "#d0e0e3",
            GrayDark = "#9eb2b8",
            GrayDarker = "#64747a",
            GrayLight = "#eBf1f2",
            GrayLighter = "#f7fAfa",
            White = "#FFFFFF",
            Success = "#127527",
            SuccessDarken = "#0d581c",
            SuccessLighten = "#179231",
            Warning = "#ca9804",
            WarningDarken = "#977203",
            WarningLighten = "#fcbe05",
            Error = "#b22c2c",
            ErrorDarken = "#8c2323",
            ErrorLighten = "#d93636",
            Info = "#038299",
            InfoDarken = "#026173",
            InfoLighten = "#04a2bf",
        },
    };
}
