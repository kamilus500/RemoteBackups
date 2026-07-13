using System.Security.Claims;
using RemoteBackups.Api.Entities;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Persistance;

namespace RemoteBackups.Api.Infrastructure.Messaging
{
    public class AuditLogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogBehavior(ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next)
        {
            var response = await next();
            
            var actionName = typeof(TRequest).Name;
            
            var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
            
            var userIdString = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var login = claimsPrincipal?.FindFirst(ClaimTypes.Name)?.Value ?? "System/Anonymous";
            
            if (string.IsNullOrEmpty(login) || login.Equals("System/Anonymous", StringComparison.InvariantCultureIgnoreCase))
            {
                var loginProperty = request.GetType().GetProperty("Login");
        
                if (loginProperty != null)
                {
                    login = loginProperty.GetValue(request)?.ToString();
                }
            }
            
            var userId = Guid.Empty;
            if (!string.IsNullOrEmpty(userIdString))
            {
                Guid.TryParse(userIdString, out userId);
            }
            
            var auditLog = new AuditLog(actionName, userId, login );

            _dbContext.AuditLogs.Add(auditLog);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}