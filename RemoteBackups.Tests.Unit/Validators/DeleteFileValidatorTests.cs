using Shouldly;
using static RemoteBackups.Api.Features.Files.Delete.DeleteFile;

namespace RemoteBackups.Tests.Unit.Validators
{
    public class DeleteFileCommandValidatorTests
    {
        private readonly DeleteFileCommandValidator _validator = new();

        [Fact]
        public void Validate_Should_ReturnSuccess_When_CommandIsValid()
        {
            var command = new DeleteFileCommand(Guid.NewGuid(), Guid.NewGuid());

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            result.Errors.ShouldBeEmpty();
        }

        [Fact]
        public void Validate_Should_ReturnFailure_When_FieldsAreEmpty()
        {
            var command = new DeleteFileCommand(Guid.Empty, Guid.Empty);

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("FileId is required.");
            result.Errors.ShouldContain("UserId is required.");
        }
    }


}