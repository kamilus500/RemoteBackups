using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RemoteBackups.Blazor.Services.Interfaces;

namespace RemoteBackups.Blazor.Pages.Files
{
    public partial class Upload
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public ILocalizationService Localizer { get; set; }

        private bool isUploading = false;
        private string progress = "0";
        private string statusMessage = "";
        private DotNetObjectReference<Upload>? objRef;

        protected override void OnInitialized()
        {
            objRef = DotNetObjectReference.Create(this);
        }

        private async Task StartUpload()
        {
            var token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            isUploading = true;
            statusMessage = "Przygotowywanie do wysyłki...";
            progress = "0";

            await JSRuntime.InvokeVoidAsync("tusInterop.uploadFile", "fileInput", "http://localhost:5150/api/files/upload", objRef, token);
        }

        [JSInvokable]
        public void OnUploadProgress(string percentage)
        {
            progress = percentage;
            StateHasChanged();
        }

        [JSInvokable]
        public void OnUploadSuccess(string fileUrl)
        {
            isUploading = false;
            statusMessage = "Plik został pomyślnie wysłany!";
            StateHasChanged();
        }

        [JSInvokable]
        public void OnUploadError(string error)
        {
            isUploading = false;
            statusMessage = $"Wystąpił błąd: {error}";
            StateHasChanged();
        }

        public void Dispose()
        {
            objRef?.Dispose();
        }
    }
}
