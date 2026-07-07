namespace RemoteBackups.Api.Features.Files.GetAll
{
    public record GetFilesRequest(
        string? SearchTerm,
        string? ContentType,
        string? SortColumn,
        string? SortOrder,
        int Page = 1,
        int PageSize = 10);
}
