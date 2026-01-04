using GS.Telescope.AlpacaClient.Dialog;
using GS.Telescope.AlpacaClient.Singletons;
using GS.Telescope.AlpacaClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GS.Telescope.AlpacaClient.MainApp
{
    public static class ServiceCollections
    {
        public static void AddCommonServices(IServiceCollection collection)
        {
            // Singleton Services
            collection.AddSingleton<MainViewModel>();
            collection.AddSingleton<SettingsService>();
            collection.AddSingleton<DialogService>();
            collection.AddSingleton<LocalizeService>();

            // Page Factory Callback
            collection.AddSingleton<Func<Type, PageViewModel>>(x => type => type switch
            {
                _ when type == typeof(HomePageViewModel) => x.GetRequiredService<HomePageViewModel>(),
                _ when type == typeof(ConnectPageViewModel) => x.GetRequiredService<ConnectPageViewModel>(),
                _ when type == typeof(HomePageViewModel) => x.GetRequiredService<HomePageViewModel>(),
                _ when type == typeof(ModelPageViewModel) => x.GetRequiredService<ModelPageViewModel>(),
                _ when type == typeof(SettingsPageViewModel) => x.GetRequiredService<SettingsPageViewModel>(),
                _ => throw new InvalidOperationException($"Page of type {type.FullName} has no view model"),
            });

            // Page Factory
            collection.AddSingleton<PageFactory>();

            // Transient Services
            collection.AddTransient<HomePageViewModel>();
            collection.AddTransient<ConnectPageViewModel>();
            collection.AddTransient<HomePageViewModel>();
            collection.AddTransient<ModelPageViewModel>();
            collection.AddTransient<SettingsPageViewModel>();
            collection.AddTransient<ConfirmDialogViewModel>();

            // Add Top Level Locator
            collection.AddTopLevelProvider();
        }
    }
}
