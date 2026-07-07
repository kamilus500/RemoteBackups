using RemoteBackups.Blazor.Models.Contracts.Files.GetAll;

namespace RemoteBackups.Blazor.Services.Interfaces
{
    public interface IFileService
    {
        Task<PagedResult<FileDto>?> GetFilesAsync(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<bool> DeleteFileAsync(Guid fileId);
        Task<Stream?> DownloadFileAsync(Guid fileId);
    }
}
