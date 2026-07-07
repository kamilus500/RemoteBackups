using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

            var tempStoragePath = Path.Combine(Path.GetTempPath(), "TusStorageTests");
            if (!Directory.Exists(tempStoragePath)) Directory.CreateDirectory(tempStoragePath);

            var fullPath = Path.Combine(tempStoragePath, tusFileId);
            var content = "Hello World";
            await File.WriteAllTextAsync(fullPath, content);

            var inMemorySettings = new Dictionary<string, string?>
            {
                {"TusSettings:StoragePath", tempStoragePath}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            try
            {
                var handler = new SaveFileMetadata.SaveFileMetadataHandler(DbContext, configuration);
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
