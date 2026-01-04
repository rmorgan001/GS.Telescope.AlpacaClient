using GS.Telescope.AlpacaClient.ViewModels;
using System;

namespace GS.Telescope.AlpacaClient.Singletons;

public class PageFactory(Func<Type, PageViewModel> factory)
{
    public PageViewModel GetPageViewModel<T>(Action<T>? afterCreation = null)
        where T : PageViewModel
    {
        var viewModel = factory(typeof(T));

        afterCreation?.Invoke((T)viewModel);

        return viewModel;
    }
}