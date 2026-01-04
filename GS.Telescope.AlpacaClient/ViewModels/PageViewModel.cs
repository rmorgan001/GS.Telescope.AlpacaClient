using CommunityToolkit.Mvvm.ComponentModel;
using GS.Telescope.AlpacaClient.MainApp;

namespace GS.Telescope.AlpacaClient.ViewModels;

public partial class PageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ApplicationPageNames _pageName;


    protected PageViewModel(ApplicationPageNames pageName)
    {
        _pageName = pageName;
    }
}