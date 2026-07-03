using Shouldly;
using static RemoteBackups.Api.Features.User.Register.RegisterUser;

namespace RemoteBackups.Tests.Unit.Validators
{
    public class RegisterUserValidatorTests
    {
        private readonly RegisterUserValidator _validator;

        public RegisterUserValidatorTests()
        {
            _validator = new RegisterUserValidator();
        }

        [Fact]
        public void Validate_Should_ReturnSuccess_When_CommandIsValid()
        {
            var command = new RegisterUserCommand("newuser", "securePassword123");

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            result.Errors.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData(null, "password")]
        public void Validate_Should_ReturnFailure_When_LoginIsMissing(string login, string password)
        {
            var result = _validator.Validate(new RegisterUserCommand(login, password));

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("Login is required.");
        }

        [Theory]
        [InlineData("user", "")]
        [InlineData("user", null)]
        public void Validate_Should_ReturnFailure_When_PasswordIsMissing(string login, string password)
        {
            var result = _validator.Validate(new RegisterUserCommand(login, password));

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain("Password is required.");
        }
    }
}