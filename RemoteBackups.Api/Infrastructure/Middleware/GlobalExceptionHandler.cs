using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RemoteBackups.Api.Infrastructure.Exceptions;

namespace RemoteBackups.Api.Infrastructure.Middleware
{
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {

            if (exception is AppException)
            {
                _logger.LogWarning(exception, "Application exception occurred: {Message}", exception.Message);
            }
            else
            {
                _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
            }

            var statusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = GetTitle(exception),
                Detail = exception.Message,
                Type = $"https://httpstatuses.com/{statusCode}",
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static string GetTitle(Exception exception) =>
            exception switch
            {
                AppException applicationException => applicationException.GetType().Name,
                _ => "Server Error"
            };
    }
}
