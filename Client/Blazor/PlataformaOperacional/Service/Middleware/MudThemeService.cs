using MudBlazor;

namespace PlataformaOperacional.Service.Middleware
{
    public class MudThemeService
    {
        private MudTheme _currentTheme;
        private readonly MudTheme _defaultTheme;

        public event Action? OnThemeChanged;

        public MudThemeService()
        {
            _defaultTheme = CreateDefaultTheme();
            _currentTheme = _defaultTheme;
        }

        private static MudTheme CreateDefaultTheme() => new()
        {
            PaletteLight = new PaletteLight
            {
                Primary = Colors.Blue.Default,
                Secondary = Colors.Teal.Accent4,
                AppbarBackground = Colors.Blue.Darken2,
                TextPrimary = Colors.Gray.Darken4
            }
        };

        public MudTheme CurrentTheme
        {
            get => _currentTheme;
            private set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    NotifyThemeChange();
                }
            }
        }

        public void SetTheme(MudTheme newTheme)
        {
            CurrentTheme = newTheme;
        }

        public void SetDefaultTheme()
        {
            SetTheme(_defaultTheme);
        }

      

        private void NotifyThemeChange()
        {
            OnThemeChanged?.Invoke();
        }
    }
}

