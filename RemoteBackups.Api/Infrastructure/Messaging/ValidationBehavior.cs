using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Infrastructure.Validations;
using RemoteBackups.Api.Infrastructure.Validations.Interfaces;

namespace RemoteBackups.Api.Infrastructure.Messaging
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
                where TRequest : IRequest<TResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationBehavior(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            Func<Task<TResponse>> next)
        {
            var validators = _serviceProvider
                .GetServices<IValidator<TRequest>>()
                .ToList();

            var errors = new List<string>();

            foreach (var validator in validators)
            {
                var result = validator.Validate(request);

                if (!result.IsValid)
                    errors.AddRange(result.Errors);
            }

            if (errors.Count > 0)
                throw new ValidationException(errors);

            return await next();
        }
    }
}
