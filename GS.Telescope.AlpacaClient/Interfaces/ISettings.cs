using Material.Colors;
using Material.Styles.Themes.Base;

namespace GS.Telescope.AlpacaClient.Interfaces
{
    public interface ISettings
    {
        string Language { get; set; }
        IBaseTheme BaseTheme { get; set; }
        PrimaryColor PrimaryColor { get; set; }
        SecondaryColor SecondaryColor { get; set; }
        string? SettingsVersion { get; set; }
        double SettingsVersionNumber { get; set; }

        /// <summary>
        /// Load settings from a json file and overwrite default properties
        /// </summary>
        void Load();
    }
}
