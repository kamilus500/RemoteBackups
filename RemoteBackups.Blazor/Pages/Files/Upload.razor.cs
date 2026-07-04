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

        private string selectedFilesInfo = "";

        private async Task HandleFileSelection(ChangeEventArgs e)
        {
            var fileNames = await JSRuntime.InvokeAsync<string[]>("eval",
                "Array.from(document.getElementById('fileInput').files).map(f => f.name)");

            selectedFilesInfo = fileNames.Length > 0
                ? $"{Localizer.GetString("Selected_files")}: {string.Join(", ", fileNames)}"
                : "";
        }

        private async Task StartUpload()
        {
            var token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            var fileNames = await JSRuntime.InvokeAsync<string[]>("eval",
                "Array.from(document.getElementById('fileInput').files).map(f => f.name)");

            isUploading = true;
            fileProgresses.Clear();
            fileStatuses.Clear();
            statusMessage = $"{Localizer.GetString("Start_uploading")} ...";

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

            if (fileStatuses.Count == fileProgresses.Count)
            {
                statusMessage = Localizer.GetString("Files_Success");
            }
            StateHasChanged();
        }

        [JSInvokable]
        public void OnUploadError(string fileName, string error)
        {
            fileStatuses[fileName] = Localizer.GetString("Error") + error;

            statusMessage = Localizer.GetString("Upload_errors");

            StateHasChanged();
        }

        public void Dispose()
        {
            objRef?.Dispose();
        }
    }
}
