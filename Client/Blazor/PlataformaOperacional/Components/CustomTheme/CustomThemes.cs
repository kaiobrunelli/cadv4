using MudBlazor;

namespace PlataformaOperacional.Componentes.CustomTheme
{
    public  class CustomThemes
    {

        public static MudTheme LightThemeCustom = new MudTheme
        {
  
            PaletteLight = new PaletteLight
            {
                Primary = "#d87b00",
                Secondary = "#005CA9",
                Tertiary = "#f39200",
                PrimaryLighten = "#000000",
                
                Background = "#ffffff",
                Surface = "#ffffff",
                AppbarBackground = "#EF765E",
                AppbarText = "#ffffff",
                DrawerBackground = "#00437A",
                DrawerText = "#ffffff",
                TextPrimary = "#404B52",
                TextSecondary = "#005CA9",
                Error = "#b22c2c",
                Success = "#0BAE10",
                Info = "#2196f3",
                Warning = "#F39200",
                Divider = "#005CA9",
                DrawerIcon = "#ffffff", 


            },
            Typography = new Typography
            {
           
                Default = new DefaultTypography()
                {
                    FontFamily = ["CAIXA STD", "Arial"]
                },                
                H1 = new H1Typography() { FontSize = "2.50rem", FontWeight ="600", LineHeight = "1.25" },
                H2 = new H2Typography() { FontSize = "2.25rem", FontWeight ="600", LineHeight = "1.25" },
                H3 = new H3Typography() { FontSize = "2rem",    FontWeight ="600", LineHeight = "1.25" },
                H4 = new H4Typography() { FontSize = "1.75rem", FontWeight ="600", LineHeight = "1.25" },
                H5 = new H5Typography() { FontSize = "1.50rem", FontWeight ="600", LineHeight = "1.25" },
                H6 = new H6Typography() { FontSize = "1.25rem", FontWeight ="600", LineHeight = "1.25" },
                
                Body1 = new Body1Typography() { FontSize = "1rem",     FontWeight = "400", LineHeight = "1.5" },
                Body2 = new Body2Typography() { FontSize = "0.875rem", FontWeight = "400", LineHeight = "1.43"},
               
            }          
           
        };
    }
}
