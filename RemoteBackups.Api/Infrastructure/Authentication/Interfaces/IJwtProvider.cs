using RemoteBackups.Api.Entities;

namespace RemoteBackups.Api.Infrastructure.Authentication.Interfaces
{
    public interface IJwtProvider
    {
        string Generate(User user);
    }
}
