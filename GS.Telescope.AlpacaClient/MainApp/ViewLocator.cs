using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GS.Telescope.AlpacaClient.ViewModels;
using System;
using System.Diagnostics.CodeAnalysis;
using DialogViewModel = GS.Telescope.AlpacaClient.Dialog.DialogViewModel;

namespace GS.Telescope.AlpacaClient.MainApp
{
    /// <summary>
    /// Given a view model, returns the corresponding view if possible.
    /// </summary>
    [RequiresUnreferencedCode(
        "Default implementation of ViewLocator involves reflection which may be trimmed away.",
        Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? param)
        {
            if (param is null)
                return null;

            var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        //public bool Match(object? data)
        //{
        //    return data is ViewModelBase;
        //}

        public bool Match(object? data) => data is PageViewModel or DialogViewModel;
    }
}
