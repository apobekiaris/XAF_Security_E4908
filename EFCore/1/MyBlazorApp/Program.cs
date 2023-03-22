using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyBlazorApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    builder.Configuration.Bind("Local", options.ProviderOptions);
});

await builder.Build().RunAsync();


public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigationManager)
        : base(provider, navigationManager)
    {
        ConfigureHandler(authorizedUrls: new[] { "https://localhost:44370" });
    
    }
}   

public class CustomAuthenticationService : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public CustomAuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Implement your logic for fetching user authentication data from an external endpoint
        // Deserialize the received data into a ClaimsPrincipal object

        return new AuthenticationState(new ClaimsPrincipal());
    }

    public async Task LoginAsync(string username, string password)
    {
        // Implement your logic for logging in with an external endpoint
        // After successful login, call NotifyAuthenticationStateChanged to update the AuthenticationState
        
    }

    public async Task LogoutAsync()
    {
        // Implement your logic for logging out from an external endpoint
        // After successful logout, call NotifyAuthenticationStateChanged to update the AuthenticationState
    }
}
