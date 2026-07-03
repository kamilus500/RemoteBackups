using RemoteBackups.Blazor.Models.Contracts.Auth.LoginUser;
using RemoteBackups.Blazor.Models.Contracts.Auth.Register;

namespace RemoteBackups.Blazor.Services.Interfaces
{
    public interface IUserService
    {
        Task<LoginUserResponse?> LoginAsync(LoginUserCommand command);
        Task<RegisterUserResponse?> RegisterAsync(RegisterUserCommand command);
    }
}
