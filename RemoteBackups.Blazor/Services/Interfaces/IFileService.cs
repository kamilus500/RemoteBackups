using RemoteBackups.Blazor.Models.Contracts.Files.GetAll;

namespace RemoteBackups.Blazor.Services.Interfaces
{
    public interface IFileService
    {
        Task<List<FileDto>?> GetFilesAsync();
        Task<bool> DeleteFileAsync(Guid fileId);
        Task<Stream?> DownloadFileAsync(Guid fileId);
    }
}
