using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RemoteBackups.Blazor;
using RemoteBackups.Blazor.Configurations;
using RemoteBackups.Blazor.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddApiClients();
builder.Services.AddAuthServices();
builder.Services.AddApplicationServices();

await builder.Build().RunAsync();
