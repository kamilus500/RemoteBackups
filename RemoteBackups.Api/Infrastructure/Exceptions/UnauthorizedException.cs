namespace RemoteBackups.Api.Infrastructure.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
