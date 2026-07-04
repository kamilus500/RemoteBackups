using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Entities;
using RemoteBackups.Api.Features.Files.GetAll;
using RemoteBackups.Tests.Integration.Fixtures;
using Shouldly;

namespace RemoteBackups.Tests.Integration.Tests.Files
{
    public class GetFilesTests : BaseIntegrationTest
    {
        public GetFilesTests(TestDatabaseContainer container) : base(container) { }

        [Fact]
        public async Task Handle_Should_ReturnOnlyUserFiles_OrderedByDateDescending()
        {
            DbContext.Users.Add(new Api.Entities.User("test", "test123"));
            await DbContext.SaveChangesAsync();

            DbContext.Users.Add(new Api.Entities.User("test2", "test123"));
            await DbContext.SaveChangesAsync();

            var users = await DbContext.Users.ToListAsync();

            var user = users.First();

            var otheruser = users.Last();

            var file1 = new FileMetaData("older.txt", "text/plain", 100, user.Id);
            file1.SetPath("path");

            var file2 = new FileMetaData("newer.txt", "text/plain", 200, user.Id);
            file2.SetPath("path2");

            var file3 = new FileMetaData("other.txt", "text/plain", 300, otheruser.Id);
            file3.SetPath("path3");

            typeof(FileMetaData).GetProperty("UploadAt")?.SetValue(file1, DateTime.UtcNow.AddDays(-1));

            DbContext.FileMetaDatas.AddRange(file1, file2, file3);
            await DbContext.SaveChangesAsync();

            var handler = new GetAllFiles.GetFilesHandler(DbContext);
            var query = new GetAllFiles.GetFilesQuery(user.Id);

            var result = await handler.Handle(query, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.First().OriginalName.ShouldBe("newer.txt");
            result.Last().OriginalName.ShouldBe("older.txt");
            result.All(f => f.Id != file3.Id).ShouldBeTrue();
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_When_UserHasNoFiles()
        {
            var handler = new GetAllFiles.GetFilesHandler(DbContext);
            var query = new GetAllFiles.GetFilesQuery(Guid.NewGuid());

            var result = await handler.Handle(query, CancellationToken.None);

            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }
    }
}
