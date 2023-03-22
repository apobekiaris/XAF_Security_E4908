using BlazorApp7;
using BlazorApp7.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddHttpClient(Options.DefaultName, c => c.BaseAddress = new Uri("https://localhost:5001/api/"))
    .ConfigurePrimaryHttpMessageHandler(provider => new AuthorizationHandler(provider.GetRequiredService<WebAPIAuthenticationStateService>()){InnerHandler = new HttpClientHandler() });
builder.Services.AddScoped<WebAPIAuthenticationStateService>();
builder.Services.AddScoped<IUserDataStorage,UserDataStorage>();
builder.Services.AddScoped<SimpleODataClientDataSource>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<WebAPIAuthenticationStateService>());

builder.Services.AddApiAuthorization();
builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});

await builder.Build().RunAsync();
