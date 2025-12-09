using System.Windows.Media;

namespace CalculatriceMargeWPF.Helpers
{
    public class Theme
    {
        public string Name { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Color PrimaryColor { get; set; }
        public Color AccentColor { get; set; }
        public Color SuccessColor { get; set; }
        public Color DangerColor { get; set; }
        public Color WarningColor { get; set; }
    }

    public class ThemeManager
    {
        private static ThemeManager _instance;
        private Theme _currentTheme;

        public static ThemeManager Instance => _instance ??= new ThemeManager();

        public event System.Action OnThemeChanged;

        public ThemeManager()
        {
            _currentTheme = GetLightTheme();
        }

        private Theme GetLightTheme()
        {
            return new Theme
            {
                Name = "Light",
                BackgroundColor = Color.FromArgb(255, 230, 234, 242),
                ForegroundColor = Color.FromArgb(255, 28, 42, 68),
                PrimaryColor = Color.FromArgb(255, 29, 53, 87),
                AccentColor = Color.FromArgb(255, 45, 156, 219),
                SuccessColor = Color.FromArgb(255, 39, 174, 96),
                DangerColor = Color.FromArgb(255, 231, 76, 60),
                WarningColor = Color.FromArgb(255, 243, 156, 18)
            };
        }

        private Theme GetDarkTheme()
        {
            return new Theme
            {
                Name = "Dark",
                BackgroundColor = Color.FromArgb(255, 15, 20, 25),
                ForegroundColor = Color.FromArgb(255, 232, 238, 247),
                PrimaryColor = Color.FromArgb(255, 100, 150, 200),
                AccentColor = Color.FromArgb(255, 100, 200, 255),
                SuccessColor = Color.FromArgb(255, 80, 200, 120),
                DangerColor = Color.FromArgb(255, 255, 100, 100),
                WarningColor = Color.FromArgb(255, 255, 200, 80)
            };
        }

        private Theme GetBlueTheme()
        {
            return new Theme
            {
                Name = "Blue",
                BackgroundColor = Color.FromArgb(255, 230, 240, 255),
                ForegroundColor = Color.FromArgb(255, 10, 40, 100),
                PrimaryColor = Color.FromArgb(255, 20, 80, 160),
                AccentColor = Color.FromArgb(255, 60, 140, 220),
                SuccessColor = Color.FromArgb(255, 100, 180, 100),
                DangerColor = Color.FromArgb(255, 220, 80, 80),
                WarningColor = Color.FromArgb(255, 240, 160, 40)
            };
        }

        private Theme GetGreenTheme()
        {
            return new Theme
            {
                Name = "Green",
                BackgroundColor = Color.FromArgb(255, 230, 245, 235),
                ForegroundColor = Color.FromArgb(255, 20, 80, 40),
                PrimaryColor = Color.FromArgb(255, 30, 120, 60),
                AccentColor = Color.FromArgb(255, 80, 200, 120),
                SuccessColor = Color.FromArgb(255, 100, 200, 100),
                DangerColor = Color.FromArgb(255, 220, 80, 80),
                WarningColor = Color.FromArgb(255, 240, 160, 40)
            };
        }

        public Theme CurrentTheme => _currentTheme;

        public void SetTheme(string themeName)
        {
            _currentTheme = themeName.ToLower() switch
            {
                "dark" => GetDarkTheme(),
                "blue" => GetBlueTheme(),
                "green" => GetGreenTheme(),
                _ => GetLightTheme()
            };
            OnThemeChanged?.Invoke();
        }

        public string[] AvailableThemes => new[] { "Light", "Dark", "Blue", "Green" };
    }
}
