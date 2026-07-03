using Shouldly;
using static RemoteBackups.Api.Features.Files.Upload.SaveFileMetadata;

namespace RemoteBackups.Tests.Unit.Validators
{
    public class SaveFileMetadataValidatorTests
    {
        private readonly SaveFileMetadataValidator _validator;

        public SaveFileMetadataValidatorTests()
        {
            _validator = new SaveFileMetadataValidator();
        }

        [Fact]
        public void Validate_Should_ReturnFailure_When_UserId_IsEmpty()
        {
            var command = new SaveFileMetadataCommand("tus-123", "file.txt", "text/plain", Guid.Empty);

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("UserId is required.");
        }

        [Fact]
        public void Validate_Should_Accumulate_Multiple_Errors()
        {
            var command = new SaveFileMetadataCommand("", "", "text/plain", Guid.Empty);

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count.ShouldBe(3);
            result.Errors.ShouldContain("TusFileId is required.");
            result.Errors.ShouldContain("Original name is required.");
            result.Errors.ShouldContain("UserId is required.");
        }
    }
}
