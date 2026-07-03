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

        public async Task<List<FileDto>?> GetFilesAsync()
        {
            return await _httpService.GetAsync<List<FileDto>>("api/files");
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
