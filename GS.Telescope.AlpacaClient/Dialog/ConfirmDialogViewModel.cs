using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GS.Telescope.AlpacaClient.Singletons;
using GS.Telescope.AlpacaClient.ViewModels;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GS.Telescope.AlpacaClient.Dialog;

public partial class ConfirmDialogViewModel(LocalizeService localizeService) : DialogViewModel
{
    [ObservableProperty] private string _title = localizeService["DiaConfirm"];
    [ObservableProperty] private string _message = localizeService["DiaMessage1"];
    [ObservableProperty] private string _acceptText = localizeService["BtnAccept"];
    [ObservableProperty] private string _cancelText = localizeService["BtnCancel"];
    [ObservableProperty] private string _iconText = "warning";
    [ObservableProperty] private int _iconHeight = 30;
    [ObservableProperty] private int _iconWidth = 30;
    [ObservableProperty] private string _statusText = "";
    [ObservableProperty] private string _progressText = "";
    [ObservableProperty] private double _dialogWidth = double.NaN;
    [ObservableProperty] private double _dialogHeight = double.NaN;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _busy;

    public bool NotBusy() => !Busy;

    [ObservableProperty] private bool _confirmed;

    [JsonIgnore]
    public Func<ConfirmDialogViewModel, Task<bool>> OnConfirm { get; set; } = (_) => Task.FromResult(true);

    [RelayCommand]
    public async Task ConfirmAsync()
    {
        if (Busy)
            return;

        Busy = true;

        // Clear status text
        StatusText = "";

        // Set initial progress text
        ProgressText = "Processing...";

        var result = await OnConfirm(this);

        Busy = false;

        if (!result)
            return;

        Confirmed = true;
        Close();
    }

    [RelayCommand(CanExecute = nameof(NotBusy))]
    private Task Cancel()
    {
        Confirmed = false;
        Close();

        return Task.CompletedTask;
    }
}