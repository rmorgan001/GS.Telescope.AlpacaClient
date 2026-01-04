using Avalonia.Platform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace GS.Telescope.AlpacaClient.Singletons
{
    public class LocalizeService : INotifyPropertyChanged
    {
        private const string IndexerName = "Item";
        private const string IndexerArrayName = "Item[]";
        private Dictionary<string, string>? _mStrings;

        public bool LoadLanguage(string language)
        {
            Language = language;

            var uri = new Uri($"avares://GS.Telescope.AlpacaClient/Assets/{language}.json");
            if (!AssetLoader.Exists(uri)) return false;
            using (var sr = new StreamReader(AssetLoader.Open(uri), Encoding.UTF8))
            {
                _mStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
            }
            Invalidate();
            return true;
        }

        public string Language { get; private set; } = "en-US";

        public string this[string key]
        {
            get
            {
                if (_mStrings != null && _mStrings.TryGetValue(key, out var res))
                    return res.Replace("\\n", "\n");

                return $"{key}"; //return $"{Language}:{key}";
            }
        }

        //public static Localizer Instance { get; set; } = new Localizer();
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Invalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerArrayName));
        }
    }
}
