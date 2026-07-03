using RemoteBackups.Api.Infrastructure;
using RemoteBackups.Api.Infrastructure.Authentication.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

var currentAssembly = typeof(Program).Assembly;
builder.Services.AddInfrastructure(builder.Configuration, currentAssembly);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapSwagger();

app.UseExceptionHandler();

app.UseCors("BlazorClientPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints(currentAssembly);

app.Run();