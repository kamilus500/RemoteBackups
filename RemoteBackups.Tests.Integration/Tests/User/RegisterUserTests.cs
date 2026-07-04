using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Features.User.Register;
using RemoteBackups.Api.Infrastructure.Exceptions;
using RemoteBackups.Tests.Integration.Fixtures;
using Shouldly;

namespace RemoteBackups.Tests.Integration.Tests.User
{
    public class RegisterUserTests : BaseIntegrationTest
    {
        public RegisterUserTests(TestDatabaseContainer container) : base(container) { }

        [Fact]
        public async Task Handle_Should_AddUserToDatabase_When_CommandIsValid()
        {
            var handler = new RegisterUser.RegisterUserHandler(DbContext);
            var command = new RegisterUser.RegisterUserCommand("newuser", "password123");

            var result = await handler.Handle(command, CancellationToken.None);

            var userInDb = await DbContext.Users.FindAsync(result.Id);
            userInDb.ShouldNotBeNull();
            userInDb.Login.ShouldBe("newuser");
        }

        [Fact]
        public async Task Handle_Should_ThrowConflictException_When_LoginAlreadyExists()
        {
            var existingUser = new Api.Entities.User("existinguser", "hashed_password");
            DbContext.Users.Add(existingUser);
            await DbContext.SaveChangesAsync();

            var handler = new RegisterUser.RegisterUserHandler(DbContext);
            var command = new RegisterUser.RegisterUserCommand("existinguser", "password123");

            await Assert.ThrowsAsync<ConflictException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_HashPasswordCorrectly()
        {
            var handler = new RegisterUser.RegisterUserHandler(DbContext);
            var password = "secretPassword";
            var command = new RegisterUser.RegisterUserCommand("newuser", password);

            var result = await handler.Handle(command, CancellationToken.None);

            var userInDb = await DbContext.Users.FindAsync(result.Id);

            userInDb.HashedPassword.ShouldNotBe(password);

            BCrypt.Net.BCrypt.Verify(password, userInDb.HashedPassword).ShouldBeTrue();
        }

        [Fact]
        public async Task Handle_Should_StoreCorrectLogin_InDatabase()
        {
            var handler = new RegisterUser.RegisterUserHandler(DbContext);
            var login = "uniqueLogin";
            var command = new RegisterUser.RegisterUserCommand(login, "password");

            await handler.Handle(command, CancellationToken.None);

            var userInDb = await DbContext.Users.SingleOrDefaultAsync(u => u.Login == login);
            userInDb.ShouldNotBeNull();
            userInDb.Login.ShouldBe(login);
        }
    }
}
