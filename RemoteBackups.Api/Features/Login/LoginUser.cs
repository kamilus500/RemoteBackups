using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Infrastructure.Authentication.Interfaces;
using RemoteBackups.Api.Infrastructure.Exceptions;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Infrastructure.Validations;
using RemoteBackups.Api.Infrastructure.Validations.Interfaces;
using RemoteBackups.Api.Persistance;

namespace RemoteBackups.Api.Features.Login
{
    public class LoginUser
    {
        public record LoginUserCommand(string Login, string Password) : IRequest<LoginUserResponse>;
        public record LoginUserResponse(string Token, string Login);

        public class LoginUserValidator : IValidator<LoginUserCommand>
        {
            public ValidationResult Validate(LoginUserCommand instance)
            {
                var errors = new List<string>();

                if (string.IsNullOrEmpty(instance.Login))
                    errors.Add("Login is required.");

                if (string.IsNullOrEmpty(instance.Password))
                    errors.Add("Password is required.");

                return errors.Count > 0
                    ? ValidationResult.Failure(errors.ToArray())
                    : ValidationResult.Success();
            }
        }

        internal sealed class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
        {
            private readonly ApplicationDbContext _context;
            private readonly IJwtProvider _jwtProvider;

            public LoginUserHandler(ApplicationDbContext context, IJwtProvider jwtProvider)
            {
                _context = context;
                _jwtProvider = jwtProvider;
            }

            public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .SingleOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

                if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword))
                {
                    throw new UnauthorizedException("Login or password are not correct.");
                }

                string token = _jwtProvider.Generate(user);

                return new LoginUserResponse(token, user.Login);
            }
        }
    }
}
