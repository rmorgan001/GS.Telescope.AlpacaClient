using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GS.Telescope.AlpacaClient.MainApp;
using GS.Telescope.AlpacaClient.Singletons;

namespace GS.Telescope.AlpacaClient.ViewModels
{
    public partial class ConnectPageViewModel(
        DialogService dialogService,
        SettingsService settingsService,
        LocalizeService localizeServic) : PageViewModel(ApplicationPageNames.Connections), IRecipient<SettingsEvent>
    {

        [ObservableProperty] private double _count;

        public void Receive(SettingsEvent message)
        {
            switch (message.PropertyName)
            {
                case "SettingsVersionNumber":
                    // MySettings.SettingsVersionNumber 
                    Count++;
                    break;

                    //default:
                    //        todo throw new Exception("Setting Not Found"); needs to log a warning
                    //    break;
            }
        }
    }
}
