using CommunityToolkit.Mvvm.ComponentModel;

namespace GS.Telescope.AlpacaClient.ViewModels
{
    public class ViewModelBase : ObservableRecipient
    {

        public ViewModelBase()
        {
            // Detect design time
            if (Avalonia.Controls.Design.IsDesignMode)
                OnDesignTimeConstructor();
        }

        protected virtual void OnDesignTimeConstructor() { }
    }
}
