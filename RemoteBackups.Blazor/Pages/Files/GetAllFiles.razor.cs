using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RemoteBackups.Blazor.Models.Contracts.Files.GetAll;
using RemoteBackups.Blazor.Services;
using RemoteBackups.Blazor.Services.Interfaces;

namespace RemoteBackups.Blazor.Pages.Files
{
    public partial class GetAllFiles
    {
        [Inject]
        public IFileService FileService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Inject]
        public ILocalizationService Localizer { get; set; }

        private List<FileDto> _files = new();
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadFiles();
        }

        private async Task LoadFiles()
        {
            _isLoading = true;
            try
            {
                var result = await FileService.GetFilesAsync();
                if (result is not null)
                {
                    _files = result;
                }
            }
            catch (Exception)
            {
                Snackbar.Add("Błąd podczas ładowania listy plików.", Severity.Error);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task DownloadFile(Guid id, string fileName)
        {
            try
            {
                var fileStream = await FileService.DownloadFileAsync(id);
                if (fileStream is not null)
                {
                    using var streamRef = new DotNetStreamReference(stream: fileStream);
                    await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
                }
                else
                {
                    Snackbar.Add("Nie udało się pobrać pliku.", Severity.Error);
                }
            }
            catch (Exception)
            {
                Snackbar.Add("Wystąpił błąd podczas pobierania.", Severity.Error);
            }
        }

        private async Task DeleteFile(Guid id)
        {
            var success = await FileService.DeleteFileAsync(id);

            if (success)
            {
                Snackbar.Add("Plik został usunięty.", Severity.Success);
                await LoadFiles();
            }
            else
            {
                Snackbar.Add("Nie udało się usunąć pliku.", Severity.Error);
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB" };
            if (bytes == 0) return "0 B";
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return $"{num} {suf[place]}";
        }
    }
}
