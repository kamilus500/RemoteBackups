namespace RemoteBackups.Api.Infrastructure.Messaging.Interfaces
{
    public interface ICommandHandler<TCommand, TResponse>
        : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    { }

    public interface ICommand<TResponse> : IRequest<TResponse> { }
}
