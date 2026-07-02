using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using System.Collections;

namespace RemoteBackups.Api.Infrastructure.Messaging
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var requestType = request.GetType();
            var responseType = typeof(TResponse);

            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            var handler = _serviceProvider.GetRequiredService(handlerType);
            var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))
                ?? throw new InvalidOperationException("Handler does not implement Handle method.");

            Func<Task<TResponse>> handlerDelegate = () =>
            {
                var task = (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;
                return task;
            };

            var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
            var enumerablePipelineType = typeof(IEnumerable<>).MakeGenericType(pipelineType);

            var pipelineInstancesObj = _serviceProvider.GetService(enumerablePipelineType);
            if (pipelineInstancesObj == null)
            {
                return await handlerDelegate();
            }

            var pipelineInstances = (IEnumerable)pipelineInstancesObj;
            var behaviors = pipelineInstances.Cast<object>().ToArray();

            Func<Task<TResponse>> pipeline = handlerDelegate;

            foreach (var behavior in behaviors.Reverse())
            {
                var localBehavior = behavior;
                var behaviorHandleMethod = pipelineType.GetMethod(nameof(IPipelineBehavior<IRequest<TResponse>, TResponse>.Handle))
                    ?? throw new InvalidOperationException("Pipeline behavior does not implement Handle method.");

                var next = pipeline;
                pipeline = () =>
                {
                    var resultTask = (Task<TResponse>)behaviorHandleMethod.Invoke(localBehavior, new object[] { request, cancellationToken, (Func<Task<TResponse>>)next })!;
                    return resultTask;
                };
            }

            return await pipeline();
        }
    }
}
