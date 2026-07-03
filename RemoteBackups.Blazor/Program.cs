using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RemoteBackups.Blazor;
using RemoteBackups.Blazor.Configurations;
using RemoteBackups.Blazor.Extensions;
using RemoteBackups.Blazor.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddLocalizations();
builder.Services.AddApiClients();
builder.Services.AddAuthServices();
builder.Services.AddApplicationServices();

var host = builder.Build();

var localizationService = host.Services.GetRequiredService<ILocalizationService>();
await localizationService.InitializeAsync();

await host.RunAsync();
