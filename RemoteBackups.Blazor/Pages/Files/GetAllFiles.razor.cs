using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RemoteBackups.Blazor.Models.Contracts.Files.GetAll;
using RemoteBackups.Blazor.Services.Interfaces;
using System.Threading;

namespace RemoteBackups.Blazor.Pages.Files
{
    public partial class GetAllFiles
    {
        [Inject] public IFileService FileService { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public ILocalizationService Localizer { get; set; }

        private MudTable<FileDto> _table;
        private string _searchString = string.Empty;

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        private async Task<TableData<FileDto>> ServerReload(TableState state, CancellationToken cancellationToken)
        {
            try
            {
                var page = state.Page + 1;
                var pageSize = state.PageSize;

                var sortColumn = state.SortLabel;
                var sortOrder = state.SortDirection == SortDirection.Descending ? "desc" : "asc";

                var result = await FileService.GetFilesAsync(_searchString, sortColumn, sortOrder, page, pageSize);

                if (result is not null)
                {
                    return new TableData<FileDto>
                    {
                        TotalItems = result.TotalCount,
                        Items = result.Items
                    };
                }
            }
            catch (Exception)
            {
                Snackbar.Add("Błąd podczas ładowania listy plików.", Severity.Error);
            }

            return new TableData<FileDto> { TotalItems = 0, Items = new List<FileDto>() };
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
                await _table.ReloadServerData();
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