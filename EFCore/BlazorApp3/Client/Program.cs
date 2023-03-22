using System.Security.Claims;
using BlazorApp3.Client;
using BlazorApp3.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("BlazorApp3.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
    .AddHttpMessageHandler<AuthorizedHandler>()
    ;

// Supply HttpClient instances that include access tokens when making requests to the server project
// builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorApp3.ServerAPI"));

builder.Services.AddApiAuthorization();
// builder.Services.AddScoped<AuthenticationStateProvider,WebAPIAuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider,WebAPIAuthenticationService1>();
builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});

await builder.Build().RunAsync();

public class WebAPIAuthenticationService1:AuthenticationStateProvider {
    public override Task<AuthenticationState> GetAuthenticationStateAsync() {
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    }
}