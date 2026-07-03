using RemoteBackups.Blazor.Models.Contracts.Auth.LoginUser;
using RemoteBackups.Blazor.Models.Contracts.Auth.Register;
using RemoteBackups.Blazor.Services.Interfaces;

namespace RemoteBackups.Blazor.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpService _httpService;

        public UserService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<LoginUserResponse?> LoginAsync(LoginUserCommand command)
        {
            return await _httpService.PostAsync<LoginUserCommand, LoginUserResponse>("api/users/login", command);
        }

        public async Task<RegisterUserResponse?> RegisterAsync(RegisterUserCommand command)
        {
            return await _httpService.PostAsync<RegisterUserCommand, RegisterUserResponse>("api/users/register", command);
        }
    }
}
