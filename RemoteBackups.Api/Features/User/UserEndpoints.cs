using RemoteBackups.Api.Infrastructure.Endpoints.Interfaces;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using static RemoteBackups.Api.Features.User.Login.LoginUser;
using static RemoteBackups.Api.Features.User.Register.RegisterUser;

namespace RemoteBackups.Api.Features.User
{
    public class UserEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/users")
               .WithTags("Users");

            group.MapPost("/login", async (LoginUserCommand command, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(command, cancellationToken);

                return Results.Ok(response);
            })
            .WithName("LoginUser")
            .Produces<LoginUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapPost("/register", async (RegisterUserCommand command, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(command, cancellationToken);

                return Results.Created($"/api/users/{response.Id}", response);
            })
            .WithName("RegisterUser")
            .Produces<RegisterUserResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
        }
    }
}
