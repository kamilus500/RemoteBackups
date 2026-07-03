using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using RemoteBackups.Blazor.Configurations;
using RemoteBackups.Blazor.Providers;
using RemoteBackups.Blazor.Services;
using RemoteBackups.Blazor.Services.Interfaces;

namespace RemoteBackups.Blazor.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddLocalizations(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            return services;
        }

        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMudServices();

            services.AddScoped<ILocalizationService, LocalizationService>();

            services.AddScoped<ILocalStorageService, LocalStorageService>();

            services.AddScoped<IHttpService, HttpService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IFileService, FileService>();

            return services;
        }

        public static IServiceCollection AddApiClients(this IServiceCollection services)
        {
            services.AddTransient<JwtInterceptor>();

            services.AddHttpClient("ApiClient", (sp, client) =>
            {
                var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(apiSettings.BaseUrl);
            })
            .AddHttpMessageHandler<JwtInterceptor>();

            services.AddScoped(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                return factory.CreateClient("ApiClient");
            });

            return services;
        }
    }
}
