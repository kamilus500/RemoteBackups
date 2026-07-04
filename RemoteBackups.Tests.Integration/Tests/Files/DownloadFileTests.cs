using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Entities;
using RemoteBackups.Api.Features.Files.Download;
using RemoteBackups.Api.Infrastructure.Exceptions;
using RemoteBackups.Tests.Integration.Fixtures;

namespace RemoteBackups.Tests.Integration.Tests.Files
{
    public class DownloadFileTests : BaseIntegrationTest
    {
        public DownloadFileTests(TestDatabaseContainer container) : base(container) { }

        [Fact]
        public async Task Handle_Should_ThrowNotFoundException_When_FileDoesNotExistInDb()
        {
            var handler = new DownloadFile.DownloadFileHandler(DbContext);
            var query = new DownloadFile.DownloadFileQuery(Guid.NewGuid(), Guid.NewGuid());

            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_ThrowNotFoundException_When_PhysicalFileIsMissing()
        {
            DbContext.Users.Add(new Api.Entities.User("test", "test123"));
            await DbContext.SaveChangesAsync();

            var user = await DbContext.Users.FirstOrDefaultAsync();
            var fileId = Guid.NewGuid();
            var metaData = new FileMetaData("test.txt", "text/plain", 10, user.Id);
            var property = typeof(FileMetaData).GetProperty("Id");
            property?.SetValue(metaData, fileId);
            metaData.SetPath("invalid_path.txt");

            DbContext.FileMetaDatas.Add(metaData);
            await DbContext.SaveChangesAsync();

            var handler = new DownloadFile.DownloadFileHandler(DbContext);
            var query = new DownloadFile.DownloadFileQuery(fileId, user.Id);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(query, CancellationToken.None));
        }
    }
}
