using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Material.Colors;
using System;

namespace GS.Telescope.AlpacaClient.Extensions;

public class PrimaryColorExt : MarkupExtension
{
    public PrimaryColorExt()
    {
    }

    public PrimaryColorExt(PrimaryColor color)
    {
        Color = color;
    }

    [ConstructorArgument("color")] public PrimaryColor Color { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new SolidColorBrush(SwatchHelper.Lookup[(MaterialColor)Color]);
    }
}