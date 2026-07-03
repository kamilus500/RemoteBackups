using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Infrastructure.Exceptions;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Infrastructure.Validations;
using RemoteBackups.Api.Infrastructure.Validations.Interfaces;
using RemoteBackups.Api.Persistance;

namespace RemoteBackups.Api.Features.Files.Download
{
    public class DownloadFile
    {
        public record DownloadFileQuery(Guid FileId, Guid UserId) : IRequest<DownloadFileResponse>;
        public record DownloadFileResponse(Stream FileStream, string ContentType, string OriginalName);

        public class DownloadFileValidator : IValidator<DownloadFileQuery>
        {
            public ValidationResult Validate(DownloadFileQuery instance)
            {
                var errors = new List<string>();

                if (instance.UserId == Guid.Empty)
                    errors.Add("UserId is required.");

                if (instance.FileId == Guid.Empty)
                    errors.Add("FileId is required.");

                return errors.Count > 0
                    ? ValidationResult.Failure(errors.ToArray())
                    : ValidationResult.Success();
            }
        }

        internal sealed class DownloadFileHandler : IRequestHandler<DownloadFileQuery, DownloadFileResponse>
        {
            private readonly ApplicationDbContext _context;

            public DownloadFileHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<DownloadFileResponse> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
            {
                var fileMetaData = await _context.FileMetaDatas
                    .AsNoTracking()
                    .SingleOrDefaultAsync(f => f.Id == request.FileId && f.UserId == request.UserId, cancellationToken);

                if (fileMetaData is null)
                {
                    throw new NotFoundException("File not found or access denied.");
                }

                if (!File.Exists(fileMetaData.Path))
                {
                    throw new NotFoundException("Physical file does not exist on the server.");
                }

                var stream = new FileStream(fileMetaData.Path, FileMode.Open, FileAccess.Read, FileShare.Read);

                return new DownloadFileResponse(stream, fileMetaData.ContentType, fileMetaData.OriginalName);
            }
        }
    }
}
