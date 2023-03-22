using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BlazorApp3.Client.Services{
    public class WebAPIAuthenticationService:RemoteAuthenticationService<RemoteAuthenticationState,RemoteUserAccount,RemoteAuthenticationUserOptions> {
        private readonly HttpClient _httpClient;

        public WebAPIAuthenticationService( IJSRuntime jsRuntime,IOptionsSnapshot<RemoteAuthenticationOptions<RemoteAuthenticationUserOptions>> options,
            NavigationManager navigation, AccountClaimsPrincipalFactory<RemoteUserAccount> accountClaimsPrincipalFactory) :
            base(jsRuntime, options, navigation, accountClaimsPrincipalFactory){
            
        }

        public override async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context){
            
            // var remoteAuthenticationResult = await base.SignInAsync(context);
            
            var authenticationState = new RemoteAuthenticationState(){ReturnUrl = "https://localhost:7156/claims-principle-data"};
            var signInAsync = new RemoteAuthenticationResult<RemoteAuthenticationState>(){State = authenticationState,Status = RemoteAuthenticationStatus.Success};
            return signInAsync;
        }
        
        
        // public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        //     var authToken = await GetTokenAsync();
        //     AccountClaimsPrincipalFactory.CreateUserAsync(new RemoteUserAccount(),new RemoteAuthenticationUserOptions(){})
        //     if (string.IsNullOrEmpty(authToken)) {
        //         return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        //     }
        //
        //     // TODO: validate the token and extract the user's authentication state
        //
        //     return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims)));
        //     // var claims = new List<Claim> {
        //     //     new(ClaimTypes.Name,"editor"),
        //     //     new("access_token", "token"),
        //     // };
        //     // // var identity = new ClaimsIdentity(claims, "apiauth_type");
        //     // var identity = new ClaimsIdentity(claims);
        //     //
        //     // // Create an authentication state from the identity
        //     // var user = new ClaimsPrincipal(identity);
        //     //
        //     // return Task.FromResult<AuthenticationState>(new AuthenticationState(user));
        // }
        public override Task<AuthenticationState> GetAuthenticationStateAsync() {
            return base.GetAuthenticationStateAsync();
        }

        protected override async ValueTask<ClaimsPrincipal> GetAuthenticatedUser() {
            var token = await JsRuntime.InvokeAsync<string>("localStorage.getItem", "user");
            return (!string.IsNullOrEmpty(token) ? JsonSerializer.Deserialize<ClaimsPrincipal>(token) : new ClaimsPrincipal(new ClaimsIdentity()))!;
        }

        public override ValueTask<AccessTokenResult> RequestAccessToken(){
            return base.RequestAccessToken();
        }

        public override ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options){
            return base.RequestAccessToken(options);
        }

        
    }
}