namespace RemoteBackups.Blazor.Models.Contracts.Files.GetAll
{
    public record FileDto(Guid Id, string OriginalName, string ContentType, long SizeInBytes, DateTime UploadAt);
}
