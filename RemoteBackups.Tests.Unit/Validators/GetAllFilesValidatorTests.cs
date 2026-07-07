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
            var query = new GetFilesQuery(Guid.NewGuid(), SearchTerm: null, ContentType: null, SortColumn: null, SortOrder: null, Page: 1, PageSize: 10);

            var result = _validator.Validate(query);

            result.IsValid.ShouldBeTrue();
            result.Errors.ShouldBeEmpty();
        }

        [Fact]
        public void Validate_Should_ReturnFailure_When_UserIdIsEmpty()
        {
            var query = new GetFilesQuery(Guid.Empty, null, null, null, null, 1, 10);

            var result = _validator.Validate(query);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("UserId is required.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Validate_Should_ReturnFailure_When_PageIsLessThanOrEqualToZero(int invalidPage)
        {
            var query = new GetFilesQuery(Guid.NewGuid(), null, null, null, null, invalidPage, 10);

            var result = _validator.Validate(query);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("Page must be greater than 0.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_Should_ReturnFailure_When_PageSizeIsLessThanOrEqualToZero(int invalidPageSize)
        {
            var query = new GetFilesQuery(Guid.NewGuid(), null, null, null, null, 1, invalidPageSize);

            var result = _validator.Validate(query);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("PageSize must be greater than 0.");
        }
    }
}