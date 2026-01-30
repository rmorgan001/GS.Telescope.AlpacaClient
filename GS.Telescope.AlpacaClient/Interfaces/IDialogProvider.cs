using DialogViewModel = GS.Telescope.AlpacaClient.Dialog.DialogViewModel;

namespace GS.Telescope.AlpacaClient.Interfaces;

public interface IDialogProvider
{
    DialogViewModel? Dialog { get; set; }
}