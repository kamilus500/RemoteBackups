using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Features.Files.Upload;
using RemoteBackups.Tests.Integration.Fixtures;
using Shouldly;

namespace RemoteBackups.Tests.Integration.Tests.Files
{
    public class SaveFileMetadataTests : BaseIntegrationTest
    {
        public SaveFileMetadataTests(TestDatabaseContainer container) : base(container) { }

        [Fact]
        public async Task Handle_Should_SaveMetadataToDatabase_When_FileExistsOnDisk()
        {
            DbContext.Users.Add(new Api.Entities.User("test", "test123"));
            await DbContext.SaveChangesAsync();

            var user = await DbContext.Users.FirstAsync();

            var tusFileId = Guid.NewGuid().ToString();
            var fileName = "test-file.txt";

            var storagePath = @"C:\TusStorage";
            if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);

            var fullPath = Path.Combine(storagePath, tusFileId);
            var content = "Hello World";
            await File.WriteAllTextAsync(fullPath, content);

            try
            {
                var handler = new SaveFileMetadata.SaveFileMetadataHandler(DbContext);
                var command = new SaveFileMetadata.SaveFileMetadataCommand(tusFileId, fileName, "text/plain", user.Id);

                var result = await handler.Handle(command, CancellationToken.None);

                result.Id.ShouldNotBe(Guid.Empty);

                var savedMetadata = await DbContext.FileMetaDatas.FindAsync(result.Id);
                savedMetadata.ShouldNotBeNull();
                savedMetadata.OriginalName.ShouldBe(fileName);
                savedMetadata.SizeInBytes.ShouldBe(content.Length);
                savedMetadata.UserId.ShouldBe(user.Id);
                savedMetadata.Path.ShouldBe(fullPath);
            }
            finally
            {
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
        }
    }
}
