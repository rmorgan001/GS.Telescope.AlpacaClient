using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace GS.Telescope.AlpacaClient.MainApp
{
    internal class Themes
    {
        public Theme CustomLight = Theme.Create(Theme.Light, LookUp(PrimaryColor.Lime), LookUp(SecondaryColor.Indigo));
        public Theme CustomDark = Theme.Create(Theme.Dark, LookUp(PrimaryColor.Cyan), LookUp(SecondaryColor.Purple));
        //public static Theme PinkGoodness = Theme.Create(Theme.Light, Colors.DeepPink, Colors.HotPink);

        //public Themes() // Brush Overrides
        //{
        //    CustomLight.Paper = Colors.AliceBlue;
        //    PinkGoodness.Paper = Colors.LightPink;
        //    PinkGoodness.PrimaryDark = Colors.Pink;
        //}

        public void Change(Theme theme)
        {
            if (Application.Current == null) return;
            var themeBootstrap = Application.Current.LocateMaterialTheme<MaterialThemeBase>();
            themeBootstrap.CurrentTheme = theme;

            var mode = theme.GetBaseThemeMode().ToString();
            Application.Current.RequestedThemeVariant = mode switch
            {
                "Dark" => ThemeVariant.Dark,
                "Light" => ThemeVariant.Light,
                _ => ThemeVariant.Default
            };
        }

        public Theme GetTheme(IBaseTheme baseTheme, PrimaryColor primary = PrimaryColor.Purple, SecondaryColor secondary = SecondaryColor.Lime)
        {
            //var a = baseShade switch
            //{
            //    "Dark" => Theme.Dark,
            //    "Light" => Theme.Light,
            //    _ => Theme.Dark
            //};

            //var convert = Enum.TryParse(primary, true, out PrimaryColor b);
            //if (!convert){b = PrimaryColor.Lime;}

            //convert = Enum.TryParse(secondary, true, out SecondaryColor c);
            //if (!convert){c = SecondaryColor.Purple;}

            var r = Theme.Create(baseTheme, LookUp(primary), LookUp(secondary));
            return r;
        }

        public static Color LookUp(PrimaryColor primaryColor)
        {
            return SwatchHelper.Lookup[(MaterialColor)primaryColor];
        }

        public static Color LookUp(SecondaryColor primaryColor)
        {
            return SwatchHelper.Lookup[(MaterialColor)primaryColor];
        }
    }
}
