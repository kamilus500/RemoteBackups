using RemoteBackups.Api.Entities;
using Shouldly;

namespace RemoteBackups.Tests.Unit.Entities
{
    public class UserTests
    {
        [Fact]
        public void Constructor_Should_Initialize_User_With_Valid_Data()
        {
            var login = "testuser";
            var hashedPassword = "hashed_password_123";

            var user = new User(login, hashedPassword);

            user.Id.ShouldNotBe(Guid.Empty);
            user.Login.ShouldBe(login);
            user.HashedPassword.ShouldBe(hashedPassword);
        }

        [Fact]
        public void GetLogin_Should_Return_Correct_Login_String()
        {
            var user = new User("johndoe", "hash");

            var result = user.GetLogin();

            result.ShouldBe("johndoe");
        }
    }
}
