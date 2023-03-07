using BlazorWASMApp.Client;
using DevExpress.Blazor.Localization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<DxLocalizationService>();
builder.Services.AddSingleton(sp => new HttpClient {  });
builder.Services.AddSingleton<WebAPIService>();

// builder.UseCors(policy => 
//     policy.WithOrigins("http://localhost:5000", "https://localhost:5001")
//         .AllowAnyMethod()
//         .WithHeaders(HeaderNames.ContentType));

var host = builder.Build();

await host.RunAsync();
