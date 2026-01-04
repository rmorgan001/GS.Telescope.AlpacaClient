using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GS.Telescope.AlpacaClient.MainApp;
using GS.Telescope.AlpacaClient.Singletons;
using GS.Telescope.AlpacaClient.ViewModels;
using GS.Telescope.AlpacaClient.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace GS.Telescope.AlpacaClient
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Register services
            var collection = new ServiceCollection();
            ServiceCollections.AddCommonServices(collection);

            // Creates a ServiceProvider containing services
            var services = collection.BuildServiceProvider();

            //theme
            var theme = new Themes();
            var settings = services.GetRequiredService<SettingsService>();
            var bColor = settings.BaseTheme;
            var pColor = settings.PrimaryColor;
            var sColor = settings.SecondaryColor;
            var newTheme = theme.GetTheme(bColor, pColor, sColor);
            theme.Change(newTheme);

            //Culture
            var local = services.GetRequiredService<LocalizeService>();
            var lang = settings.Language;
            local.LoadLanguage(lang);
            // Localizer.Instance.LoadLanguage(lang);

            switch (ApplicationLifetime)
            {
                case IClassicDesktopStyleApplicationLifetime desktop:
                    // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                    DisableAvaloniaDataAnnotationValidation();

                    desktop.MainWindow = new MainWindow
                    {
                        DataContext = services.GetRequiredService<MainViewModel>()
                    };
                    break;
                case ISingleViewApplicationLifetime singleViewPlatform:
                    singleViewPlatform.MainView = new MainWindow()
                    {
                        DataContext = services.GetRequiredService<MainViewModel>()
                    };
                    break;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}