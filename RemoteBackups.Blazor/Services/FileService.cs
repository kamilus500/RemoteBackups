using RemoteBackups.Blazor.Models.Contracts.Files.GetAll;
using RemoteBackups.Blazor.Services.Interfaces;

namespace RemoteBackups.Blazor.Services
{
    public class FileService : IFileService
    {
        private readonly IHttpService _httpService;
        private readonly HttpClient _httpClient;

        public FileService(IHttpService httpService, HttpClient httpClient)
        {
            _httpService = httpService;
            _httpClient = httpClient;
        }

        public async Task<PagedResult<FileDto>?> GetFilesAsync(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

            if (!string.IsNullOrWhiteSpace(sortColumn))
                queryParams.Add($"sortColumn={Uri.EscapeDataString(sortColumn)}");

            if (!string.IsNullOrWhiteSpace(sortOrder))
                queryParams.Add($"sortOrder={sortOrder}");

            var queryString = string.Join("&", queryParams);

            return await _httpService.GetAsync<PagedResult<FileDto>>($"api/files?{queryString}");
        }

        public async Task<bool> DeleteFileAsync(Guid fileId)
        {
            try
            {
                await _httpService.DeleteAsync($"api/files/{fileId}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Stream?> DownloadFileAsync(Guid fileId)
        {
            var response = await _httpClient.GetAsync($"api/files/{fileId}/download", HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }

            return null;
        }
    }
}
