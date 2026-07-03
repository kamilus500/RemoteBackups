using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using RemoteBackups.Blazor.Configurations;
using RemoteBackups.Blazor.Providers;
using RemoteBackups.Blazor.Services;
using RemoteBackups.Blazor.Services.Interfaces;

namespace RemoteBackups.Blazor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ILocalStorageService, LocalStorageService>();

            services.AddScoped<IHttpService, HttpService>();

            services.AddScoped<IUserService, UserService>();

            return services;
        }

        public static IServiceCollection AddApiClients(this IServiceCollection services)
        {
            services.AddScoped(sp =>
            {
                var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;

                return new HttpClient
                {
                    BaseAddress = new Uri(apiSettings.BaseUrl)
                };
            });

            return services;
        }
    }
}
