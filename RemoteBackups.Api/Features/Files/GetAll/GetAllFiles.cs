using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Infrastructure.Validations;
using RemoteBackups.Api.Infrastructure.Validations.Interfaces;
using RemoteBackups.Api.Persistance;

namespace RemoteBackups.Api.Features.Files.GetAll
{
    public class GetAllFiles
    {
        public record GetFilesQuery(
            Guid UserId,
            string? SearchTerm,
            string? ContentType,
            string? SortColumn,
            string? SortOrder,
            int Page,
            int PageSize) : IRequest<PagedResult<FileDto>>;

        public record FileDto(Guid Id, string OriginalName, string ContentType, long SizeInBytes, DateTime UploadAt);

        public record PagedResult<T>(List<T> Items, int Page, int PageSize, int TotalCount)
        {
            public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
            public bool HasNextPage => Page * PageSize < TotalCount;
            public bool HasPreviousPage => Page > 1;
        }

        public class GetAllFilesValidator : IValidator<GetFilesQuery>
        {
            public ValidationResult Validate(GetFilesQuery instance)
            {
                var errors = new List<string>();

                if (instance.UserId == Guid.Empty)
                    errors.Add("UserId is required.");

                if (instance.Page <= 0)
                    errors.Add("Page must be greater than 0.");

                if (instance.PageSize <= 0)
                    errors.Add("PageSize must be greater than 0.");

                return errors.Count > 0
                    ? ValidationResult.Failure(errors.ToArray())
                    : ValidationResult.Success();
            }
        }

        internal sealed class GetFilesHandler : IRequestHandler<GetFilesQuery, PagedResult<FileDto>>
        {
            private readonly ApplicationDbContext _context;

            public GetFilesHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<PagedResult<FileDto>> Handle(GetFilesQuery request, CancellationToken cancellationToken)
            {
                var query = _context.FileMetaDatas
                    .AsNoTracking()
                    .Where(f => f.UserId == request.UserId);

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(f => f.OriginalName.Contains(request.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(request.ContentType))
                {
                    query = query.Where(f => f.ContentType == request.ContentType);
                }

                query = (request.SortColumn?.ToLower(), request.SortOrder?.ToLower()) switch
                {
                    ("name", "asc") => query.OrderBy(f => f.OriginalName),
                    ("name", "desc") => query.OrderByDescending(f => f.OriginalName),
                    ("type", "asc") => query.OrderBy(f => f.ContentType),
                    ("type", "desc") => query.OrderByDescending(f => f.ContentType),
                    ("size", "asc") => query.OrderBy(f => f.SizeInBytes),
                    ("size", "desc") => query.OrderByDescending(f => f.SizeInBytes),
                    (_, "asc") => query.OrderBy(f => f.UploadAt),
                    _ => query.OrderByDescending(f => f.UploadAt)
                };

                int totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(f => new FileDto(f.Id, f.OriginalName, f.ContentType, f.SizeInBytes, f.UploadAt))
                    .ToListAsync(cancellationToken);

                return new PagedResult<FileDto>(items, request.Page, request.PageSize, totalCount);
            }
        }
    }
}