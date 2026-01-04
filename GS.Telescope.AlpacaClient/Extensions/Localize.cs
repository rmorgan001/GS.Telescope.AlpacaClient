using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using GS.Telescope.AlpacaClient.Singletons;
using System;
using System.Diagnostics.CodeAnalysis;


namespace GS.Telescope.AlpacaClient.Extensions
{
    public class Localize : MarkupExtension
    {
        public Localize(string key)
        {
            Key = key;
        }

        public string Key { get; set; }

        public static LocalizeService? Localizer { get; set; }

        public string? Context { get; set; }

        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicProperties, typeof(LocalizeService))]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var keyToUse = Key;
            if (!string.IsNullOrWhiteSpace(Context))
                keyToUse = $"{Context}/{Key}";

            //var local = services.GetRequiredService<LocalizeService>();

            var binding = new ReflectionBindingExtension($"[{keyToUse}]")
            {
                Mode = BindingMode.OneWay,
                Source = Localizer  //Source = Localizer.Instance,
                                    // Source = LocalizeService.Instance, //Source = Localizer.Instance,

            };

            return binding.ProvideValue(serviceProvider);
        }
    }
}
