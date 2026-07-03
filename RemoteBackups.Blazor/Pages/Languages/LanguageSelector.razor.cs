using Microsoft.AspNetCore.Components;
using RemoteBackups.Blazor.Services.Interfaces;

namespace RemoteBackups.Blazor.Pages.Languages
{
    public partial class LanguageSelector
    {
        private string _currentCulture = "pl-PL";

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var savedCulture = await LocalStorage.GetItemAsync("culture");
            if (!string.IsNullOrEmpty(savedCulture))
            {
                _currentCulture = savedCulture;
            }
        }

        private async Task OnLanguageChanged(string selectedCulture)
        {
            if (!string.IsNullOrEmpty(selectedCulture))
            {
                await LocalStorage.SetItemAsync("culture", selectedCulture);
                NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
            }
        }
    }
}
