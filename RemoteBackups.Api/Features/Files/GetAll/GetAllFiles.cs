using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Infrastructure.Validations;
using RemoteBackups.Api.Infrastructure.Validations.Interfaces;
using RemoteBackups.Api.Persistance;

namespace RemoteBackups.Api.Features.Files.GetAll
{
    public class GetAllFiles
    {
        public record GetFilesQuery(Guid UserId) : IRequest<List<FileDto>>;
        public record FileDto(Guid Id, string OriginalName, string ContentType, long SizeInBytes, DateTime UploadAt);

        public class SaveFileMetadataValidator : IValidator<GetFilesQuery>
        {
            public ValidationResult Validate(GetFilesQuery instance)
            {
                var errors = new List<string>();

                if (instance.UserId == Guid.Empty)
                    errors.Add("UserId is required.");

                return errors.Count > 0
                    ? ValidationResult.Failure(errors.ToArray())
                    : ValidationResult.Success();
            }
        }

        internal sealed class GetFilesHandler : IRequestHandler<GetFilesQuery, List<FileDto>>
        {
            private readonly ApplicationDbContext _context;

            public GetFilesHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<FileDto>> Handle(GetFilesQuery request, CancellationToken cancellationToken)
                => await _context.FileMetaDatas
                    .AsNoTracking()
                    .Where(f => f.UserId == request.UserId)
                    .OrderByDescending(f => f.UploadAt)
                    .Select(f => new FileDto(f.Id, f.OriginalName, f.ContentType, f.SizeInBytes, f.UploadAt))
                    .ToListAsync(cancellationToken);
        }
    }
}
