using BlazorWebAssembly;
using BlazorWebAssembly.Pages;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<Login.AuthenticationState>();
builder.Services.AddScoped(_ => new HttpClient(){BaseAddress = new Uri("https://localhost:5001/api/")});
builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});
builder.Services.AddScoped<AuthenticationStateProvider,MyAuthenticationStateProvider>();
await builder.Build().RunAsync();

public class MyAuthenticationStateProvider:AuthenticationStateProvider {
    public override Task<AuthenticationState> GetAuthenticationStateAsync() {
        throw new NotImplementedException();
    }
}