using Shouldly;
using static RemoteBackups.Api.Features.Files.Download.DownloadFile;

namespace RemoteBackups.Tests.Unit.Validators
{
    public class DownloadFileValidatorTests
    {
        private readonly DownloadFileValidator _validator = new();

        [Fact]
        public void Validate_Should_ReturnSuccess_When_QueryIsValid()
        {
            var query = new DownloadFileQuery(Guid.NewGuid(), Guid.NewGuid());

            var result = _validator.Validate(query);

            result.IsValid.ShouldBeTrue();
            result.Errors.ShouldBeEmpty();
        }

        [Fact]
        public void Validate_Should_ReturnFailure_When_FieldsAreEmpty()
        {
            var query = new DownloadFileQuery(Guid.Empty, Guid.Empty);

            var result = _validator.Validate(query);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("UserId is required.");
            result.Errors.ShouldContain("FileId is required.");
        }
    }
}
