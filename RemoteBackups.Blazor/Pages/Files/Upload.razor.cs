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
        private string statusMessage = "";
        private DotNetObjectReference<Upload>? objRef;
        private Dictionary<string, string> fileProgresses = new();
        private Dictionary<string, string> fileStatuses = new();
        protected override void OnInitialized()
        {
            objRef = DotNetObjectReference.Create(this);
        }

        private async Task StartUpload()
        {
            var token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            var fileNames = await JSRuntime.InvokeAsync<string[]>("eval",
                "Array.from(document.getElementById('fileInput').files).map(f => f.name)");

            isUploading = true;
            fileProgresses.Clear();
            fileStatuses.Clear();

            foreach (var name in fileNames)
            {
                fileProgresses[name] = "0";
            }

            await JSRuntime.InvokeVoidAsync("tusUpload.uploadFiles", "fileInput", "http://localhost:5150/api/files/upload", objRef, token);
        }

        [JSInvokable]
        public void OnUploadProgress(string fileName, string percentage)
        {
            fileProgresses[fileName] = percentage;
            StateHasChanged();
        }

        [JSInvokable]
        public void OnUploadSuccess(string fileName)
        {
            fileStatuses[fileName] = Localizer.GetString("Ready");
            StateHasChanged();
        }

        [JSInvokable]
        public void OnUploadError(string fileName, string error)
        {
            fileStatuses[fileName] = Localizer.GetString("Error") + error;
            StateHasChanged();
        }

        public void Dispose()
        {
            objRef?.Dispose();
        }
    }
}
