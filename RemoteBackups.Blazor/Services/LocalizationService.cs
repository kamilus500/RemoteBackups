using RemoteBackups.Blazor.Services.Interfaces;
using System.Globalization;

namespace RemoteBackups.Blazor.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILocalStorageService _localStorage;
        private CultureInfo _currentCulture;

        public LocalizationService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task InitializeAsync()
        {
            var savedCulture = await _localStorage.GetItemAsync("culture");
            _currentCulture = new CultureInfo(savedCulture ?? "pl-PL");

            CultureInfo.DefaultThreadCurrentCulture = _currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = _currentCulture;
        }

        public string GetString(string key)
        {
            return Resources.Resources.ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? $"[{key}]";
        }

        public string GetCurrentCulture() => CultureInfo.CurrentUICulture.Name;
    }
}
