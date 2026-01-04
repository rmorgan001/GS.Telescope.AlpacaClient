using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GS.Telescope.AlpacaClient.Dialog;
using GS.Telescope.AlpacaClient.MainApp;
using GS.Telescope.AlpacaClient.Singletons;
using Material.Colors;
using Material.Styles.Themes;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace GS.Telescope.AlpacaClient.ViewModels
{
    public partial class SettingsPageViewModel(
        MainViewModel mainViewModel,
        DialogService dialogService,
        SettingsService settingsService,
        LocalizeService localizeService) : PageViewModel(ApplicationPageNames.Settings)
    {

        #region Theme

        [ObservableProperty] private bool _selectedBaseTheme = settingsService.BaseTheme == Theme.Dark;

        [ObservableProperty] private PrimaryColor _selectedPrimaryColor = settingsService.PrimaryColor;

        [ObservableProperty] private SecondaryColor _selectedSecondaryColor = settingsService.SecondaryColor;

        [ObservableProperty] private ObservableCollection<PrimaryColor> _primaryColors = new(Enum.GetValues<PrimaryColor>());

        [ObservableProperty] private ObservableCollection<SecondaryColor> _secondaryColors = new(Enum.GetValues<SecondaryColor>());

        [RelayCommand] private void TriggerTheme()
        {
            var a = SelectedBaseTheme ? Theme.Dark : Theme.Light;
            settingsService.BaseTheme = a;
            settingsService.PrimaryColor = SelectedPrimaryColor;
            settingsService.SecondaryColor = SelectedSecondaryColor;

            var theme = new Themes();
            var newTheme = theme.GetTheme(a, SelectedPrimaryColor, SelectedSecondaryColor);
            theme.Change(newTheme);
        }

        #endregion

        #region Languages

        [ObservableProperty] private string[] _languageList = ["de-DE", "en-US"];

        [ObservableProperty] private string _selectedLanguage = settingsService.Language;

        [RelayCommand] private async Task TestDialog1Async()
        {
            await TriggerLanguage1Async();
        }

        private async Task<bool> TriggerLanguage1Async(bool warn = true)
        {
            if (warn)
            {
                var confirmViewModel = new ConfirmDialogViewModel(localizeService)
                {
                    IconText = "Info",
                    IconHeight = 50,
                    IconWidth = 50
                };

                await dialogService.ShowDialog(mainViewModel, confirmViewModel);

                // Ignore if we clicked cancel
                if (!confirmViewModel.Confirmed)
                    return false;
            }
            settingsService.Language = SelectedLanguage;
            localizeService.LoadLanguage(SelectedLanguage);
            return true;
        }
        [RelayCommand] private async Task TriggerLanguageAsync()
        {
            var confirmViewModel = new ConfirmDialogViewModel(localizeService)
            {
                IconHeight = 50,
                IconWidth = 50,
            };
            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return;

            settingsService.Language = SelectedLanguage;
            localizeService.LoadLanguage(SelectedLanguage);
        }

        #endregion
    }
}
