using BlazorApp6.Client;
using BlazorApp6.Client.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Security.Claims;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("BlazorApp6.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:5001/api/"))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorApp6.ServerAPI"));
builder.Services.AddScoped<WebAPIAuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<WebAPIAuthenticationService>());
builder.Services.AddApiAuthorization(options => {
    options = null;
});
builder.Services.AddAuthorizationCore(options => {
    // options.FallbackPolicy = new AuthorizationPolicyBuilder()
    //     .RequireAuthenticatedUser()
    //     .Build();
    options.AddPolicy("BlockAllPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });
    // string claimType = ClaimTypes.Name;
    // var claimsAuthorizationRequirement = new ClaimsAuthorizationRequirement(claimType,null);
    // options.AddPolicy("AuthUsers", policyBuilder => {
    //     policyBuilder.AddRequirements(claimsAuthorizationRequirement);
    // } );
});
builder.Services.AddScoped<WebAPILogin.User>();

builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});
await builder.Build().RunAsync();


