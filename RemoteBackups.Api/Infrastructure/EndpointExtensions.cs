using RemoteBackups.Api.Infrastructure.Endpoints.Interfaces;
using System.Reflection;

namespace RemoteBackups.Api.Infrastructure
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app, Assembly assembly)
        {
            var endpointTypes = assembly.GetTypes()
                .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in endpointTypes)
            {
                var endpoint = (IEndpoint)Activator.CreateInstance(type)!;
                endpoint.MapEndpoint(app);
            }

            return app;
        }
    }
}
