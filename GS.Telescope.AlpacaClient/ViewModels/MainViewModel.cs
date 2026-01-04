using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GS.Telescope.AlpacaClient.Extensions;
using GS.Telescope.AlpacaClient.Interfaces;
using GS.Telescope.AlpacaClient.MainApp;
using GS.Telescope.AlpacaClient.Models;
using GS.Telescope.AlpacaClient.Singletons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GS.Telescope.AlpacaClient.ViewModels
{
    public partial class MainViewModel : PageViewModel, IRecipient<SettingsEvent>, IDialogProvider
    {
        private readonly PageFactory _factory;
        private readonly LocalizeService _localizeService;

        [ObservableProperty] private bool _isPaneOpen;
        [ObservableProperty] private MainMenuItemTemplate? _selectedListItem;
        [ObservableProperty] private DialogViewModel? _dialog;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConnectPageIsActive))]
        [NotifyPropertyChangedFor(nameof(HomePageIsActive))]
        [NotifyPropertyChangedFor(nameof(ModelPageIsActive))]
        [NotifyPropertyChangedFor(nameof(SettingsPageIsActive))]
        private PageViewModel _currentPage;

        public required ObservableCollection<MainMenuItemTemplate> Items { get; set; }
        public bool ConnectPageIsActive => CurrentPage.PageName == ApplicationPageNames.Connections;
        public bool HomePageIsActive => CurrentPage.PageName == ApplicationPageNames.Home;
        public bool ModelPageIsActive => CurrentPage.PageName == ApplicationPageNames.Model3D;
        public bool SettingsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Settings;

        public MainViewModel(PageFactory pageFactory,
            LocalizeService localizeService) : base(ApplicationPageNames.Settings)
        {
            Messenger.RegisterAll(this);

            _localizeService = localizeService;
            _factory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));

            Localize.Localizer = localizeService; // sent to Localize extension for the amaxl pages 
            CurrentPage = _factory.GetPageViewModel<ConnectPageViewModel>();
            UpdateMainMenuNames();
        }

        public void UpdateMainMenuNames()
        {
            var menu = new List<MainMenuItemTemplate>()
            {
                new(typeof(ConnectPageViewModel), "LanConnect", "Connect", _localizeService["Connections"]),
                new(typeof(HomePageViewModel), "Home", "Home", _localizeService["Home"]),
                new(typeof(ModelPageViewModel), "Rotate3d", "Model", _localizeService["3D Model"]),
                new(typeof(SettingsPageViewModel), "Cog", "Settings", _localizeService["Settings"])
            };
            Items = new ObservableCollection<MainMenuItemTemplate>(menu);
            OnPropertyChanged(nameof(Items));
        }

        public void Receive(SettingsEvent message)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            //if (MySettings == null) { return;}

            switch (message.PropertyName)
            {
                case "BaseTheme":
                    //BaseTheme = MySettings.BaseTheme;
                    // BaseTheme = _settings.BaseTheme;
                    break;
                case "SettingsVersion":
                    //SettingsVersion = _settings.SettingsVersion;
                    break;
                case "SettingsVersionNumber":
                    //SettingsVersionNumber = _settings.SettingsVersionNumber;
                    break;
                    //default:
                    //        todo throw new Exception("Setting Not Found"); needs to log a warning
                    //    break;
            }
        }

        [RelayCommand]
        private void TriggerPane()
        {
            UpdateMainMenuNames();
            IsPaneOpen = !IsPaneOpen;
        }

        [RelayCommand]
        private void SelectedItemChanged()
        {
            var a = SelectedListItem;
            if (a == null) return;

            CurrentPage = a.Name switch
            {
                "Connect" => _factory.GetPageViewModel<ConnectPageViewModel>(),
                "Home" => _factory.GetPageViewModel<HomePageViewModel>(),
                "Model" => _factory.GetPageViewModel<ModelPageViewModel>(),
                "Settings" => _factory.GetPageViewModel<SettingsPageViewModel>(),
                _ => CurrentPage
            };
        }

    }
}
