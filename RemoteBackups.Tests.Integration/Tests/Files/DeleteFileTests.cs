using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Entities;
using RemoteBackups.Api.Features.Files.Delete;
using RemoteBackups.Api.Infrastructure.Exceptions;
using RemoteBackups.Tests.Integration.Fixtures;
using Shouldly;

namespace RemoteBackups.Tests.Integration.Tests.Files
{
    public class DeleteFileTests : BaseIntegrationTest
    {
        public DeleteFileTests(TestDatabaseContainer container) : base(container) { }

        [Fact]
        public async Task Handle_Should_RemoveMetadataAndFile_When_FileExists()
        {
            DbContext.Users.Add(new Api.Entities.User("test", "test123"));
            await DbContext.SaveChangesAsync();

            var user = await DbContext.Users.FirstOrDefaultAsync();
            var fileId = Guid.NewGuid();
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
            await File.WriteAllTextAsync(tempPath, "Content to delete");

            var metaData = new FileMetaData("delete.txt", "text/plain", 17, user.Id);
            var property = typeof(FileMetaData).GetProperty("Id");
            property?.SetValue(metaData, fileId);
            metaData.SetPath(tempPath);

            DbContext.FileMetaDatas.Add(metaData);
            await DbContext.SaveChangesAsync();

            var handler = new DeleteFile.DeleteFileHandler(DbContext);
            var command = new DeleteFile.DeleteFileCommand(fileId, user.Id);

            var result = await handler.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();

            var deletedEntry = await DbContext.FileMetaDatas.FindAsync(fileId);
            deletedEntry.ShouldBeNull();

            File.Exists(tempPath).ShouldBeFalse();
        }

        [Fact]
        public async Task Handle_Should_ThrowNotFoundException_When_FileDoesNotExistInDb()
        {
            var handler = new DeleteFile.DeleteFileHandler(DbContext);
            var command = new DeleteFile.DeleteFileCommand(Guid.NewGuid(), Guid.NewGuid());

            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_RemoveMetadata_EvenIfPhysicalFileDoesNotExist()
        {
            DbContext.Users.Add(new Api.Entities.User("test", "test123"));
            await DbContext.SaveChangesAsync();

            var user = await DbContext.Users.FirstOrDefaultAsync();

            var fileId = Guid.NewGuid();
            var metaData = new FileMetaData("ghost.txt", "text/plain", 0, user.Id);
            var property = typeof(FileMetaData).GetProperty("Id");
            property?.SetValue(metaData, fileId);
            metaData.SetPath("non_existent_path.txt");

            DbContext.FileMetaDatas.Add(metaData);
            await DbContext.SaveChangesAsync();

            var handler = new DeleteFile.DeleteFileHandler(DbContext);
            var command = new DeleteFile.DeleteFileCommand(fileId, user.Id);

            await handler.Handle(command, CancellationToken.None);

            var deletedEntry = await DbContext.FileMetaDatas.FindAsync(fileId);
            deletedEntry.ShouldBeNull();
        }
    }
}
