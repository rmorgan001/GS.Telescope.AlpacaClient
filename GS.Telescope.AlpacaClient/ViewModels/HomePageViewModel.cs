using GS.Telescope.AlpacaClient.MainApp;
using GS.Telescope.AlpacaClient.Singletons;

namespace GS.Telescope.AlpacaClient.ViewModels
{
    public class HomePageViewModel(
        MainViewModel mainViewModel,
        DialogService dialogService,
        SettingsService settingsService,
        LocalizeService localizeServic) : PageViewModel(ApplicationPageNames.Connections)
    {

    }
}
