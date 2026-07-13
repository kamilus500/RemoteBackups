using RemoteBackups.Api.Entities;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Infrastructure.Validations;
using RemoteBackups.Api.Infrastructure.Validations.Interfaces;
using RemoteBackups.Api.Persistance;

namespace RemoteBackups.Api.Features.Files.Upload
{
    public class SaveFileMetadata
    {
        public record SaveFileMetadataCommand(
            string TusFileId,
            string OriginalName,
            string ContentType,
            Guid UserId) : ICommand<SaveFileMetadataResponse>;

        public record SaveFileMetadataResponse(Guid Id);

        public class SaveFileMetadataValidator : IValidator<SaveFileMetadataCommand>
        {
            public ValidationResult Validate(SaveFileMetadataCommand instance)
            {
                var errors = new List<string>();

                if (string.IsNullOrEmpty(instance.TusFileId))
                    errors.Add("TusFileId is required.");

                if (string.IsNullOrEmpty(instance.OriginalName))
                    errors.Add("Original name is required.");

                if (instance.UserId == Guid.Empty)
                    errors.Add("UserId is required.");

                return errors.Count > 0
                    ? ValidationResult.Failure(errors.ToArray())
                    : ValidationResult.Success();
            }
        }

        internal sealed class SaveFileMetadataHandler : IRequestHandler<SaveFileMetadataCommand, SaveFileMetadataResponse>
        {
            private readonly ApplicationDbContext _context;
            private readonly string _storagePath;

            public SaveFileMetadataHandler(ApplicationDbContext context, IConfiguration configuration)
            {
                _context = context;
                _storagePath = configuration["TusSettings:StoragePath"] ?? @"C:\TusStorage";
            }

            public async Task<SaveFileMetadataResponse> Handle(SaveFileMetadataCommand request, CancellationToken cancellationToken)
            {
                var physicalPath = Path.Combine(_storagePath, request.TusFileId);

                long sizeInBytes = new FileInfo(physicalPath).Length;

                var fileMetaData = new FileMetaData(request.OriginalName, request.ContentType, sizeInBytes, request.UserId);

                fileMetaData.SetPath(physicalPath);

                _context.FileMetaDatas.Add(fileMetaData);
                await _context.SaveChangesAsync(cancellationToken);

                return new SaveFileMetadataResponse(fileMetaData.Id);
            }
        }
    }
}
