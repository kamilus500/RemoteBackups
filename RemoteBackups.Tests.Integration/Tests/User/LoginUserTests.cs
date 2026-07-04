using Moq;
using RemoteBackups.Api.Features.User.Login;
using RemoteBackups.Api.Infrastructure.Authentication.Interfaces;
using RemoteBackups.Api.Infrastructure.Exceptions;
using RemoteBackups.Tests.Integration.Fixtures;
using Shouldly;

namespace RemoteBackups.Tests.Integration.Tests.User
{
    public class LoginUserTests : BaseIntegrationTest
    {
        private readonly Mock<IJwtProvider> _jwtProviderMock;

        public LoginUserTests(TestDatabaseContainer container) : base(container)
        {
            _jwtProviderMock = new Mock<IJwtProvider>();
        }

        [Fact]
        public async Task Handle_Should_ReturnToken_When_CredentialsAreValid()
        {
            var login = "testuser";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new Api.Entities.User(login, hashedPassword);

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var expectedToken = "mocked-jwt-token";
            _jwtProviderMock
                .Setup(x => x.Generate(It.IsAny<Api.Entities.User>()))
                .Returns(expectedToken);

            var handler = new LoginUser.LoginUserHandler(DbContext, _jwtProviderMock.Object);
            var command = new LoginUser.LoginUserCommand(login, password);

            var result = await handler.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Token.ShouldBe(expectedToken);
            result.Login.ShouldBe(login);
        }

        [Fact]
        public async Task Handle_Should_ThrowUnauthorizedException_When_UserDoesNotExist()
        {
            var handler = new LoginUser.LoginUserHandler(DbContext, _jwtProviderMock.Object);
            var command = new LoginUser.LoginUserCommand("nonexistent", "password");

            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_ThrowUnauthorizedException_When_PasswordIsIncorrect()
        {
            var login = "testuser";
            var user = new Api.Entities.User(login, BCrypt.Net.BCrypt.HashPassword("correct_password"));

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var handler = new LoginUser.LoginUserHandler(DbContext, _jwtProviderMock.Object);
            var command = new LoginUser.LoginUserCommand(login, "wrong_password");

            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}