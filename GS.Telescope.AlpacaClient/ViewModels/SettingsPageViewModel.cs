using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GS.Telescope.AlpacaClient.Dialog;
using GS.Telescope.AlpacaClient.MainApp;
using GS.Telescope.AlpacaClient.Singletons;
using Material.Colors;
using Material.Styles.Themes;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;



namespace GS.Telescope.AlpacaClient.ViewModels
{
    public partial class SettingsPageViewModel(MainViewModel mainViewModel , DialogService dialogService, SettingsService settingsService, LocalizeService localizeService, string? blank) : PageViewModel(ApplicationPageNames.Settings)
    {
        // ReSharper disable once UnusedMember.Global
        public string? Blank { get; } = blank;


        public SettingsPageViewModel(MainViewModel mainViewModel , DialogService dialogService, SettingsService settingsService, LocalizeService localizeService) : this(mainViewModel, dialogService, settingsService, localizeService, null)
        {
            GetObjFileNames();
        }

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

        //[RelayCommand] private async Task TestDialog1Async()
        //{
        //    await TriggerLanguage1Async();
        //}

        //private async Task TriggerLanguage1Async(bool warn = true)
        //{
        //    if (warn)
        //    {
        //        var confirmViewModel = new ConfirmDialogViewModel(localizeService)
        //        {
        //            IconText = "Info",
        //            IconHeight = 50,
        //            IconWidth = 50
        //        };

        //        await dialogService.ShowDialog(mainViewModel, confirmViewModel);

        //        // Ignore if we clicked cancel
        //        if (!confirmViewModel.Confirmed) return;
        //    }
        //    settingsService.Language = SelectedLanguage;
        //    localizeService.LoadLanguage(SelectedLanguage);
        //}

        [RelayCommand]
        private async Task TriggerLanguageAsync()
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
            mainViewModel.UpdateMenuCommand.Execute(null);
        }
        #endregion

        #region Model

        [ObservableProperty] private ObservableCollection<string> _modelFileNames = [];

        [ObservableProperty] private string _selectedModel = settingsService.ModelFilename;

        [RelayCommand]
        private async Task TriggerModelAsync()
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

            settingsService.ModelFilename = SelectedModel;
        }

        private void GetObjFileNames()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            var objFiles = Directory.GetFiles(filePath,"*.obj");
            var objFilenames = new ObservableCollection<string>();
            foreach (var filename in objFiles){objFilenames.Add(Path.GetFileName(filename));}
            ModelFileNames = objFilenames;
        }

        #endregion
    }
}
