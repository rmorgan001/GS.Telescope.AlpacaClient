using GS.Telescope.AlpacaClient.ViewModels;

namespace GS.Telescope.AlpacaClient.Interfaces;

public interface IDialogProvider
{
    DialogViewModel? Dialog { get; set; }
}