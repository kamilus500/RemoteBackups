using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Entities;
using RemoteBackups.Api.Infrastructure.Exceptions;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Infrastructure.Validations;
using RemoteBackups.Api.Infrastructure.Validations.Interfaces;
using RemoteBackups.Api.Persistance;

namespace RemoteBackups.Api.Features.User.Register
{
    public class RegisterUser
    {
        public record RegisterUserCommand(string Login, string Password) : IRequest<RegisterUserResponse>;
        public record RegisterUserResponse(Guid Id, string Login);

        public class RegisterUserValidator : IValidator<RegisterUserCommand>
        {
            public ValidationResult Validate(RegisterUserCommand instance)
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

        internal sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
        {
            private readonly ApplicationDbContext _context;

            public RegisterUserHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                bool userExists = await _context.Users
                    .AnyAsync(u => u.Login == request.Login, cancellationToken);

                if (userExists)
                {
                    throw new ConflictException("User with this login already exists.");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new Entities.User(request.Login, hashedPassword);

                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                return new RegisterUserResponse(user.Id, user.Login);
            }
        }
    }
}
