using Shouldly;
using static RemoteBackups.Api.Features.Files.GetAll.GetAllFiles;

namespace RemoteBackups.Tests.Unit.Validators
{
    public class GetAllFilesValidatorTests
    {
        private readonly GetAllFilesValidator _validator = new();

        [Fact]
        public void Validate_Should_ReturnSuccess_When_QueryIsValid()
        {
            var query = new GetFilesQuery(Guid.NewGuid());

            var result = _validator.Validate(query);

            result.IsValid.ShouldBeTrue();
            result.Errors.ShouldBeEmpty();
        }

        [Fact]
        public void Validate_Should_ReturnFailure_When_UserIdIsEmpty()
        {
            var query = new GetFilesQuery(Guid.Empty);

            var result = _validator.Validate(query);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("UserId is required.");
        }
    }
}
