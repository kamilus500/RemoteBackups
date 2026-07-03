using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Infrastructure.Exceptions;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Infrastructure.Validations;
using RemoteBackups.Api.Infrastructure.Validations.Interfaces;
using RemoteBackups.Api.Persistance;

namespace RemoteBackups.Api.Features.Files.Delete
{
    public class DeleteFile
    {
        public record DeleteFileCommand(Guid FileId, Guid UserId) : IRequest<DeleteFileResponse>;

        public record DeleteFileResponse();

        public class SaveFileMetadataValidator : IValidator<DeleteFileCommand>
        {
            public ValidationResult Validate(DeleteFileCommand instance)
            {
                var errors = new List<string>();

                if (instance.FileId == Guid.Empty)
                    errors.Add("FileId is required.");

                if (instance.UserId == Guid.Empty)
                    errors.Add("UserId is required.");

                return errors.Count > 0
                    ? ValidationResult.Failure(errors.ToArray())
                    : ValidationResult.Success();
            }
        }

        internal sealed class DeleteFileHandler : IRequestHandler<DeleteFileCommand, DeleteFileResponse>
        {
            private readonly ApplicationDbContext _context;

            public DeleteFileHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<DeleteFileResponse> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
            {
                var fileMetaData = await _context.FileMetaDatas
                    .SingleOrDefaultAsync(f => f.Id == request.FileId && f.UserId == request.UserId, cancellationToken);

                if (fileMetaData is null)
                {
                    throw new NotFoundException("File not found or access denied.");
                }

                _context.FileMetaDatas.Remove(fileMetaData);
                await _context.SaveChangesAsync(cancellationToken);

                if (File.Exists(fileMetaData.Path))
                {
                    File.Delete(fileMetaData.Path);
                }

                return new DeleteFileResponse();
            }
        }
    }
}
