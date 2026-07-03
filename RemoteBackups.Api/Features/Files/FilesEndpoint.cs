using RemoteBackups.Api.Infrastructure.Endpoints.Interfaces;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using System.Security.Claims;
using tusdotnet;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Stores;
using static RemoteBackups.Api.Features.Files.Delete.DeleteFile;
using static RemoteBackups.Api.Features.Files.Download.DownloadFile;
using static RemoteBackups.Api.Features.Files.GetAll.GetAllFiles;
using static RemoteBackups.Api.Features.Files.Upload.SaveFileMetadata;

namespace RemoteBackups.Api.Features.Files
{
    public class FilesEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/files")
                           .WithTags("Files")
                           .RequireAuthorization();

            group.MapGet("", async (ClaimsPrincipal user, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var response = await mediator.Send(new GetFilesQuery(userId), cancellationToken);
                return Results.Ok(response);
            })
            .WithName("GetFiles")
            .Produces<List<FileDto>>(StatusCodes.Status200OK);

            group.MapGet("{fileId:guid}/download", async (Guid fileId, ClaimsPrincipal user, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var response = await mediator.Send(new DownloadFileQuery(fileId, userId), cancellationToken);

                return Results.File(response.FileStream, response.ContentType, response.OriginalName);
            })
            .WithName("DownloadFile")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("{fileId:guid}", async (Guid fileId, ClaimsPrincipal user, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await mediator.Send(new DeleteFileCommand(fileId, userId), cancellationToken);

                return Results.NoContent();
            })
            .WithName("DeleteFile")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            group.MapTus("/upload", async httpContext => new DefaultTusConfiguration
            {
                Store = new TusDiskStore(@"C:\TusStorage"),
                MaxAllowedUploadSizeInBytes = 1024 * 1024 * 1024,
                Events = new Events
                {
                    OnFileCompleteAsync = async eventContext =>
                    {
                        var fileId = eventContext.FileId;
                        var file = await ((ITusReadableStore)eventContext.Store).GetFileAsync(fileId, eventContext.CancellationToken);
                        var metadata = await file.GetMetadataAsync(eventContext.CancellationToken);

                        var originalFileName = metadata.ContainsKey("filename")
                            ? metadata["filename"].GetString(System.Text.Encoding.UTF8)
                            : "Unknown";

                        var contentType = metadata.ContainsKey("filetype")
                            ? metadata["filetype"].GetString(System.Text.Encoding.UTF8)
                            : "application/octet-stream";

                        var userIdClaim = eventContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (!Guid.TryParse(userIdClaim, out var userId))
                        {
                            userId = Guid.Empty;
                        }

                        var mediator = eventContext.HttpContext.RequestServices.GetRequiredService<IMediator>();
                        var command = new SaveFileMetadataCommand(fileId, originalFileName, contentType, userId);

                        await mediator.Send(command, eventContext.CancellationToken);
                    }
                }
            }).RequireAuthorization();
        }
    }
}
