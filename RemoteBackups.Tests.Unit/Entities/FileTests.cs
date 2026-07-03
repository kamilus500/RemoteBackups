using RemoteBackups.Api.Entities;
using Shouldly;

namespace RemoteBackups.Tests.Unit.Entities
{
    public class FileMetaDataTests
    {
        [Fact]
        public void Constructor_Should_Initialize_Properties_And_Set_UploadAt()
        {
            var originalName = "document.pdf";
            var contentType = "application/pdf";
            var sizeInBytes = 1024L;
            var userId = Guid.NewGuid();

            var file = new FileMetaData(originalName, contentType, sizeInBytes, userId);

            file.OriginalName.ShouldBe(originalName);
            file.ContentType.ShouldBe(contentType);
            file.SizeInBytes.ShouldBe(sizeInBytes);
            file.UserId.ShouldBe(userId);
            file.Path.ShouldBeNull();
        }

        [Fact]
        public void SetPath_Should_Update_Path_Property()
        {
            var file = new FileMetaData("test.txt", "text/plain", 100, Guid.NewGuid());
            var expectedPath = @"C:\TusStorage\some-guid-id";

            file.SetPath(expectedPath);

            file.Path.ShouldBe(expectedPath);
        }
    }
}
