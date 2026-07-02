namespace RemoteBackups.Api.Infrastructure.Authentication.Models
{
    public class JwtOptions
    {
        public string SecretKey { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int ExpiryInMinutes { get; init; }
    }
}
