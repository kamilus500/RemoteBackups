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

            var query = new GetAllFiles.GetFilesQuery(user.Id, null, null, null, null, 1, 10);

            var result = await handler.Handle(query, CancellationToken.None);

            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.First().OriginalName.ShouldBe("newer.txt");
            result.Items.Last().OriginalName.ShouldBe("older.txt");
            result.Items.Any(f => f.Id == file3.Id).ShouldBeFalse();
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_When_UserHasNoFiles()
        {
            var handler = new GetAllFiles.GetFilesHandler(DbContext);
            var query = new GetAllFiles.GetFilesQuery(Guid.NewGuid(), null, null, null, null, 1, 10);

            var result = await handler.Handle(query, CancellationToken.None);

            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(0);
            result.Items.ShouldBeEmpty();
        }

        [Fact]
        public async Task Handle_Should_FilterBySearchTerm_Correctly()
        {
            DbContext.Users.Add(new Api.Entities.User("searchUser", "test123"));
            await DbContext.SaveChangesAsync();
            var user = await DbContext.Users.FirstAsync();

            var file1 = new FileMetaData("document.pdf", "application/pdf", 500, user.Id);
            file1.SetPath("path");

            var file2 = new FileMetaData("image.png", "image/png", 600, user.Id);
            file2.SetPath("path");
            DbContext.FileMetaDatas.AddRange(file1, file2);
            await DbContext.SaveChangesAsync();

            var handler = new GetAllFiles.GetFilesHandler(DbContext);

            var query = new GetAllFiles.GetFilesQuery(user.Id, SearchTerm: "doc", null, null, null, 1, 10);

            var result = await handler.Handle(query, CancellationToken.None);

            result.TotalCount.ShouldBe(1);
            result.Items.Single().OriginalName.ShouldBe("document.pdf");
        }

        [Fact]
        public async Task Handle_Should_PaginateCorrectly_BasedOnPageAndPageSize()
        {
            DbContext.Users.Add(new Api.Entities.User("pageUser", "test123"));
            await DbContext.SaveChangesAsync();
            var user = await DbContext.Users.FirstAsync();

            var file1 = new FileMetaData("file1.txt", "text/plain", 100, user.Id);
            file1.SetPath("path");
            var file2 = new FileMetaData("file2.txt", "text/plain", 200, user.Id);
            file2.SetPath("path");
            var file3 = new FileMetaData("file3.txt", "text/plain", 300, user.Id);
            file3.SetPath("path");

            typeof(FileMetaData).GetProperty("UploadAt")?.SetValue(file3, DateTime.UtcNow.AddMinutes(10));
            typeof(FileMetaData).GetProperty("UploadAt")?.SetValue(file2, DateTime.UtcNow.AddMinutes(5));
            typeof(FileMetaData).GetProperty("UploadAt")?.SetValue(file1, DateTime.UtcNow);

            DbContext.FileMetaDatas.AddRange(file1, file2, file3);
            await DbContext.SaveChangesAsync();

            var handler = new GetAllFiles.GetFilesHandler(DbContext);

            var query = new GetAllFiles.GetFilesQuery(user.Id, null, null, null, null, Page: 2, PageSize: 1);

            var result = await handler.Handle(query, CancellationToken.None);

            result.TotalCount.ShouldBe(3);
            result.Items.Count.ShouldBe(1);
            result.Items.Single().OriginalName.ShouldBe("file2.txt");
            result.HasNextPage.ShouldBeTrue();
            result.HasPreviousPage.ShouldBeTrue();
        }
    }
}