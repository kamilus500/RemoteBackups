using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace RemoteBackups.Api.Infrastructure.Messaging
{
    public class PipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
                       where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PipelineBehavior<TRequest, TResponse>> _logger;

        public PipelineBehavior(ILogger<PipelineBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next)
        {
            var sw = Stopwatch.StartNew();
            string requestPayload;

            try
            {
                requestPayload = JsonSerializer.Serialize(request);
            }
            catch
            {
                requestPayload = request?.ToString() ?? "<null>";
            }

            _logger.LogInformation("Start handling {RequestType}. Payload: {Payload}", typeof(TRequest).FullName, requestPayload);

            try
            {
                var response = await next();
                sw.Stop();
                _logger.LogInformation("Handled {RequestType} in {ElapsedMs} ms", typeof(TRequest).FullName, sw.ElapsedMilliseconds);
                return response;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                sw.Stop();
                _logger.LogInformation("Handling of {RequestType} was cancelled after {ElapsedMs} ms", typeof(TRequest).FullName, sw.ElapsedMilliseconds);
                throw;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Unhandled exception while handling {RequestType} after {ElapsedMs} ms", typeof(TRequest).FullName, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
