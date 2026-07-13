using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RemoteBackups.Api.Infrastructure.Authentication;
using RemoteBackups.Api.Infrastructure.Authentication.Interfaces;
using RemoteBackups.Api.Infrastructure.Authentication.Models;
using RemoteBackups.Api.Infrastructure.Messaging;
using RemoteBackups.Api.Infrastructure.Messaging.Interfaces;
using RemoteBackups.Api.Infrastructure.Middleware;
using RemoteBackups.Api.Infrastructure.Validations.Interfaces;
using RemoteBackups.Api.Persistance;
using System.Reflection;
using System.Text;
namespace RemoteBackups.Api.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly featureAssembly)
    {
        services.AddHttpContextAccessor();
        
        services.AddCors(options =>
        {
            options.AddPolicy("BlazorClientPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });

            options.AddPolicy("TusCorsPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:5177")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithExposedHeaders(
                          "Tus-Resumable",
                          "Tus-Version",
                          "Tus-Extension",
                          "Tus-Max-Size",
                          "Upload-Length",
                          "Upload-Offset",
                          "Location",
                          "Upload-Metadata",
                          "Upload-Defer-Length",
                          "Upload-Concat");
            });
        });

        services.AddPersistance(configuration);
        services.AddMediator(featureAssembly);
        services.AddValidators(featureAssembly);
        services.AddServices();
        services.AddAuth(configuration);
        services.AddSwaggerInfra();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    public static IServiceCollection AddMediator(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediator, Mediator>();

        var handlers = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Select(i => new { Implementation = t, Interface = i }))
            .Where(x =>
                x.Interface.IsGenericType &&
                (
                    x.Interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                    x.Interface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                ))
            .ToList();

        foreach (var handler in handlers)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services, Assembly assembly)
    {
        var validatorTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IValidator<>))
                .Select(i => new { Service = i, Implementation = t }))
            .ToList();

        foreach (var v in validatorTypes)
        {
            services.AddScoped(v.Service, v.Implementation);
        }

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        return services;
    }

    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("conString")
            ?? throw new InvalidOperationException("Connection string is missing in configuration.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()
                    ?? throw new InvalidOperationException("JWT options are missing.");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddSwaggerInfra(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Remote backups API",
                Version = "v1",
                Description = "Remote backups API"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Wpisz 'Bearer' [spacja] a potem swój token."
            });

            c.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
        });

        return services;
    }

    public static WebApplication MapSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Remote Backups API v1");
            c.RoutePrefix = "swagger";
        });

        return app;
    }
}