using CommunityToolkit.Mvvm.ComponentModel;
using GreenSwampWebView;
using GS.Telescope.AlpacaClient.MainApp;
using GS.Telescope.AlpacaClient.Singletons;
using Material.Colors;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace GS.Telescope.AlpacaClient.ViewModels
{
    public partial class ModelPageViewModel(MainViewModel mainViewModel , DialogService dialogService, SettingsService settingsService, LocalizeService localizeService, string? blank) : PageViewModel(ApplicationPageNames.Model3D)
    {
        // ReSharper disable once UnusedMember.Global
        public string? Blank { get; } = blank;

        private bool _isViewerInitialized;
        private readonly string? _currentObjFilePath;

        public ModelPageViewModel(MainViewModel mainViewModel , DialogService dialogService, SettingsService settingsService, LocalizeService localizeService) : this(mainViewModel, dialogService, settingsService, localizeService, null)
        {
            try
            {
                // Load the viewer HTML
                WebViewControl.NavigationCompleted += WebView_NavigationCompleted;
                WebViewControl.NavigationStarting += WebView_NavigationStarting;
                WebViewControl.NavigationFailed += WebView_NavigationFailed;

                var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Index1.html");
                if (File.Exists(htmlPath))
                {
                    WebViewControl.Navigate($"file:///{htmlPath.Replace("\\", "/")}");
                }

                // Load the OTA
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", settingsService.ModelFilename);
                if (!File.Exists(filePath)) return;
                _currentObjFilePath = filePath;
            }
            catch (Exception ex)
            {
                StatusTextBlock = $"Error loading model: {ex.Message}";
            }
        }

        [ObservableProperty] private WebView _webViewControl = new(); 

        // Telescope Control (Primary Controls)
        [ObservableProperty]  private double _altitude = 30.0;  // 0-90°

        [ObservableProperty] private double _rightAscension;  // 0-360°

        [ObservableProperty] private double _declination = 270.0;  // 0-360°

        // Hemisphere Selection
        [ObservableProperty] private bool _isNorthernHemisphere = true;

        [ObservableProperty] private bool _isSouthernHemisphere;

        // Model Information
        [ObservableProperty] private string _modelFileName = "No telescope loaded";

        [ObservableProperty] private bool _isModelLoaded;

        [ObservableProperty] private string _secondaryColor = "#00FF00";

        [ObservableProperty] private string _primaryColor = "#FF0000";

        // UI Labels & Help Text
        [ObservableProperty] private string _hemisphereDescription = "Northern Hemisphere";

        [ObservableProperty] private string _altitudeHelperText = "0° = Horizon, 90° = Zenith";

        [ObservableProperty] private string _raHelperText = "0° = Vernal Equinox, increases eastward";

        [ObservableProperty] private string _decHelperText = "0° = Celestial Equator, ±90° = Poles";

        [ObservableProperty] private string _statusTextBlock = "Ready";
        
        // Property Change Handlers
        partial void OnIsNorthernHemisphereChanged(bool value)
        {
            if (!value) return;
            IsSouthernHemisphere = false;
            HemisphereDescription = "Northern Hemisphere";
        }

        partial void OnIsSouthernHemisphereChanged(bool value)
        {
            if (!value) return;
            IsNorthernHemisphere = false;
            HemisphereDescription = "Southern Hemisphere";
        }

        private void WebView_NavigationStarting(object? sender, WebViewNavigationEventArgs e)
        {
            StatusTextBlock = "Initializing 3D viewer...";
        }

        private async void WebView_NavigationCompleted(object? sender, WebViewNavigationEventArgs e)
        {
            if (e.IsSuccess)
            {
                StatusTextBlock = "3D viewer loaded successfully";
                await InitializeBabylonViewerAsync();
            }
            else
            {
                StatusTextBlock = "Failed to load 3D viewer";
            }
        }

        private void WebView_NavigationFailed(object? sender, WebViewNavigationEventArgs e)
        {
            StatusTextBlock = $"Navigation failed: {e.ErrorMessage}";
        }

        private async Task InitializeBabylonViewerAsync()
        {
            try
            {
                // get material theme colors
                var primary = settingsService.PrimaryColor ;
                var primaryColorHex =  SwatchHelper.Lookup[(MaterialColor)primary].ToString().Remove(1,2);
                
                var secondary = settingsService.SecondaryColor ;
                var secondaryColorHex =  SwatchHelper.Lookup[(MaterialColor)secondary].ToString().Remove(1,2);

                StatusTextBlock = "Loading textures...";
                //System.Diagnostics.Debug.WriteLine("System Diagnostics Debug");

                // Load texture images as base64 (same pattern as OBJ loading)
                var assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");

                var skyboxPath = Path.Combine(assetsPath, "NorthAmerica.jpg");
                var compassNorthPath = Path.Combine(assetsPath, "compassN.png");
                var compassSouthPath = Path.Combine(assetsPath, "compassS.png");

                string skyboxBase64 = "";
                string compassNorthBase64 = "";
                string compassSouthBase64 = "";

                if (File.Exists(skyboxPath))
                {
                    byte[] skyboxBytes = await File.ReadAllBytesAsync(skyboxPath);
                    skyboxBase64 = Convert.ToBase64String(skyboxBytes);
                }

                if (File.Exists(compassNorthPath))
                {
                    byte[] compassNorthBytes = await File.ReadAllBytesAsync(compassNorthPath);
                    compassNorthBase64 = Convert.ToBase64String(compassNorthBytes);
                }

                if (File.Exists(compassSouthPath))
                {
                    byte[] compassSouthBytes = await File.ReadAllBytesAsync(compassSouthPath);
                    compassSouthBase64 = Convert.ToBase64String(compassSouthBytes);
                }

                StatusTextBlock = "Initializing 3D viewer...";

                // Set texture data as global JavaScript variables (safer than passing as function parameters)
                await WebViewControl.ExecuteScriptAsync($"window.skyboxTextureData = '{skyboxBase64}';");
                await WebViewControl.ExecuteScriptAsync($"window.compassNorthTextureData = '{compassNorthBase64}';");
                await WebViewControl.ExecuteScriptAsync($"window.compassSouthTextureData = '{compassSouthBase64}';");
                await WebViewControl.ExecuteScriptAsync($"window.primaryHexColorData = '{primaryColorHex}';");
                await WebViewControl.ExecuteScriptAsync($"window.secondaryHexColorData = '{secondaryColorHex}';");
                await WebViewControl.ExecuteScriptAsync($"window.nHemiSphereData = '{settingsService.NHemiSphere.ToString()}';");

                // Initialize viewer (it will read from window variables)
                var result = await WebViewControl.ExecuteScriptAsync("initViewer()");
                _isViewerInitialized = result.Contains("initialized") || result.Contains("Viewer");

                if (_isViewerInitialized)
                {
                    StatusTextBlock = "TelescopeViewer initialized. Load a telescope model to begin.";
                    if (_currentObjFilePath != null) _ = LoadModelAsync(_currentObjFilePath);
                    IsNorthernHemisphere = settingsService.NHemiSphere;
                    await UpdateHemisphereAsync();
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock = $"Viewer initialization error: {ex.Message}";
            }
        }

        private async Task LoadModelAsync(string objFilePath)
        {
            try
            {
                if (!_isViewerInitialized)
                {
                    StatusTextBlock = "Error: Viewer not initialized";
                    return;
                }

                if (!File.Exists(objFilePath))
                {
                    StatusTextBlock = "Error: OBJ file not found";
                    return;
                }

                StatusTextBlock = "Reading OBJ file...";

                // Read OBJ file content
                var objBytes = await File.ReadAllBytesAsync(objFilePath);
                var objBase64 = Convert.ToBase64String(objBytes);

                // Check for accompanying MTL file
                string? mtlBase64 = null;
                var mtlFilePath = Path.ChangeExtension(objFilePath, ".mtl");
                
                if (File.Exists(mtlFilePath))
                {
                    StatusTextBlock = "Reading MTL file...";
                    var mtlBytes = await File.ReadAllBytesAsync(mtlFilePath);
                    mtlBase64 = Convert.ToBase64String(mtlBytes);
                }

                StatusTextBlock = "Transferring model to 3D viewer...";

                // Call JavaScript to load the model
                var script = mtlBase64 != null
                    ? $"(async () => {{ return await loadModel('{objBase64}', '{mtlBase64}'); }})()"
                    : $"(async () => {{ return await loadModel('{objBase64}', null); }})()";

                Debug.WriteLine("Executing model load script...");
                var result = await WebViewControl.ExecuteScriptAsync(script);
                Debug.WriteLine($"Load result: {result}");

                // Check result
                if (string.IsNullOrEmpty(result))
                {
                    StatusTextBlock = "Warning: No response from viewer";
                    IsModelLoaded = true;
                }
                else if (result.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    StatusTextBlock = $"Telescope loaded: {Path.GetFileName(objFilePath)}";
                    IsModelLoaded = true;
                    ModelFileName = Path.GetFileName(objFilePath);

                    await Task.Delay(100);
                    await UpdateTelescopeRotationAsync();
                }
                else if (result.Contains("Error", StringComparison.OrdinalIgnoreCase))
                {
                    StatusTextBlock = $"Failed to load model: {result}";
                    IsModelLoaded = false;
                }
                else
                {
                    StatusTextBlock = $"Telescope loaded: {Path.GetFileName(objFilePath)}";
                    IsModelLoaded = true;
                    ModelFileName = Path.GetFileName(objFilePath);

                    await Task.Delay(100);
                    await UpdateTelescopeRotationAsync();
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock = $"Error loading model: {ex.Message}";
                IsModelLoaded = false;
            }
        }

        private async Task UpdateTelescopeRotationAsync()
        {
            try
            {
                if (!IsModelLoaded)
                {
                    return;
                }

                double alt = Altitude;
                double ra = RightAscension;
                double dec = Declination;

                string script = $"setTelescopeRotation({alt.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {ra.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {dec.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
            
                Debug.WriteLine($"Calling: {script}");
                var result = await WebViewControl.ExecuteScriptAsync(script);
                Debug.WriteLine($"Result: {result}");

                StatusTextBlock = $"Alt={alt:F1}°, RA={ra:F1}°, Dec={dec:F1}°";
            }
            catch (Exception ex)
            {
                StatusTextBlock = $"Error updating rotation: {ex.Message}";
                Debug.WriteLine($"Error in UpdateTelescopeRotationAsync: {ex}");
            }
        }

        private async Task UpdateHemisphereAsync()
        {
            try
            {
                bool isNorthern = IsNorthernHemisphere;
                string script = $"setHemisphere({isNorthern.ToString().ToLower()})";
            
                Debug.WriteLine($"Calling: {script}");
                var result = await WebViewControl.ExecuteScriptAsync(script);
                Debug.WriteLine($"Result: {result}");
            }
            catch (Exception ex)
            {
                StatusTextBlock = $"Error updating hemisphere: {ex.Message}";
                Debug.WriteLine($"Error in UpdateHemisphereAsync: {ex}");
            }
        }

        //private async void HemisphereChanged(object? sender, RoutedEventArgs e)
        //{
        //    if (!_isViewerInitialized) return;
        //    await UpdateHemisphereAsync();
        //    StatusTextBlock = $"Hemisphere: {HemisphereDescription}";
        //}
    }
}
