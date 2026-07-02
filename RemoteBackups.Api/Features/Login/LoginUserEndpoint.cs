using RemoteBackups.Api.Infrastructure.Endpoints.Interfaces;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using static RemoteBackups.Api.Features.Login.LoginUser;

namespace RemoteBackups.Api.Features.Login
{
    public class LoginUserEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/users/login", async (LoginUserCommand command, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(command, cancellationToken);

                return Results.Ok(response);
            })
            .WithName("LoginUser")
            .WithTags("Users")
            .Produces<LoginUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
