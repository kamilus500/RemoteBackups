using Shouldly;
using static RemoteBackups.Api.Features.User.Login.LoginUser;

namespace RemoteBackups.Tests.Unit.Validators
{
    public class LoginUserValidatorTests
    {
        private readonly LoginUserValidator _validator;

        public LoginUserValidatorTests()
        {
            _validator = new LoginUserValidator();
        }

        [Fact]
        public void Validate_Should_ReturnSuccess_When_CommandIsValid()
        {
            var command = new LoginUserCommand("admin", "secret123");

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            result.Errors.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData(null, "password")]
        public void Validate_Should_ReturnFailure_When_LoginIsMissing(string login, string password)
        {
            var command = new LoginUserCommand(login, password);

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("Login is required.");
        }

        [Theory]
        [InlineData("admin", "")]
        [InlineData("admin", null)]
        public void Validate_Should_ReturnFailure_When_PasswordIsMissing(string login, string password)
        {
            var command = new LoginUserCommand(login, password);

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("Password is required.");
        }
    }
}
