using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GS.Telescope.AlpacaClient.Interfaces;
using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GS.Telescope.AlpacaClient.Singletons
{

    public class SettingsService : ObservableRecipient, ISettings
    {

        private string _language = "en-US";
        public string Language
        {
            get => _language;
            set => SetValue(ref _language, value);
        }

        private bool _nHemiSphere = true;
        public bool NHemiSphere
        {
            get => _nHemiSphere;
            set => SetValue(ref _nHemiSphere, value);
        }

        private string _modelFilename = "Default.obj";
        public string ModelFilename
        {
            get => _modelFilename;
            set => SetValue(ref _modelFilename, value);
        }

        private IBaseTheme _baseTheme = Theme.Dark;
        public IBaseTheme BaseTheme
        {
            get => _baseTheme;
            set => SetValue(ref _baseTheme, value);
        }
        
        private PrimaryColor _primaryColor = PrimaryColor.Purple;
        public PrimaryColor PrimaryColor
        {
            get => _primaryColor;
            set => SetValue(ref _primaryColor, value);
        }

        private SecondaryColor _secondaryColor = SecondaryColor.Lime;
        public SecondaryColor SecondaryColor
        {
            get => _secondaryColor;
            set => SetValue(ref _secondaryColor, value);
        }

        private string? _settingsVersion = "1.0.0.0";
        public string? SettingsVersion
        {
            get => _settingsVersion;
            set => SetValue(ref _settingsVersion, value);
        }

        private double _settingsVersionNumber;
        public double SettingsVersionNumber
        {
            get => _settingsVersionNumber;
            set => SetValue(ref _settingsVersionNumber, value);
        }

        protected void SetValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            Messenger.Send(new SettingsEvent(propertyName));
        }

        /// <summary>
        /// Load settings from a json file and overwrite default properties
        /// </summary>
        public void Load()
        {
            SettingsVersionNumber = 1.0;
            //todo compare assembly versions and decide what to do
        }

    }

    /// <summary>
    /// Event used for the INotifyPropertyChanged
    /// </summary>
    /// <param name="PropertyName"></param>
    public record SettingsEvent(string? PropertyName);
}
