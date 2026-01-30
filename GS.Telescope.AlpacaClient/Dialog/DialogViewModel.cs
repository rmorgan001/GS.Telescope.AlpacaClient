using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using GS.Telescope.AlpacaClient.ViewModels;

namespace GS.Telescope.AlpacaClient.Dialog;

public partial class DialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isDialogOpen;

    protected TaskCompletionSource CloseTask = new();

    public async Task WaitAsnyc()
    {
        await CloseTask.Task;
    }

    public void Show()
    {
        if (CloseTask.Task.IsCompleted)
            CloseTask = new TaskCompletionSource();
        IsDialogOpen = true;
    }

    public void Close()
    {
        IsDialogOpen = false;

        CloseTask.TrySetResult();
    }

}