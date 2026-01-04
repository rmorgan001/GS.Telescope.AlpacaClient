using GS.Telescope.AlpacaClient.Dialog;
using GS.Telescope.AlpacaClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GS.Telescope.AlpacaClient.MainApp;

public static class DesignData
{
    private static readonly IServiceProvider Services;

    static DesignData()
    {
        var collection = new ServiceCollection();

        // Register the same services as the application
        ServiceCollections.AddCommonServices(collection);

        // Register design-time only services
        Services = collection.BuildServiceProvider();
    }

    public static MainViewModel MainViewModel => Services.GetRequiredService<MainViewModel>();
    public static ConnectPageViewModel ConnectPageViewModel => Services.GetRequiredService<ConnectPageViewModel>();
    public static HomePageViewModel HomePageViewModel => Services.GetRequiredService<HomePageViewModel>();
    public static ModelPageViewModel ModelPageViewModel => Services.GetRequiredService<ModelPageViewModel>();
    public static SettingsPageViewModel SettingsPageViewModel => Services.GetRequiredService<SettingsPageViewModel>();
    public static ConfirmDialogViewModel ConfirmDialogViewModel => Services.GetRequiredService<ConfirmDialogViewModel>();
}