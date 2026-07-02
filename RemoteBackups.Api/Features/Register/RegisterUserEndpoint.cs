using RemoteBackups.Api.Infrastructure.Endpoints.Interfaces;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using static RemoteBackups.Api.Features.Register.RegisterUser;

namespace RemoteBackups.Api.Features.Register
{
    public class RegisterUserEndpoint : IEndpoint
    {
        public RegisterUserEndpoint()
        {
        }

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/users/register", async (RegisterUserCommand command, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(command, cancellationToken);

                return Results.Created($"/api/users/{response.Id}", response);
            })
            .WithName("RegisterUser")
            .WithTags("Users")
            .Produces<RegisterUserResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
        }
    }
}
