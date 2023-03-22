using System.Globalization;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using BlazorApp6.Client.Pages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BlazorApp6.Client{
    public class WebAPIAuthenticationService:RemoteAuthenticationService<RemoteAuthenticationState,RemoteUserAccount,RemoteAuthenticationUserOptions> {
        private readonly WebAPILogin.User _user;
        private readonly HttpClient _httpClient;
        public class Claim:System.Security.Claims.Claim {
            public Claim(string type, string value) : base(type, value) {
            }
        }
        public WebAPIAuthenticationService(IHttpClientFactory clientFactory,WebAPILogin.User user, IJSRuntime jsRuntime,IOptionsSnapshot<RemoteAuthenticationOptions<RemoteAuthenticationUserOptions>> options,
            NavigationManager navigation, AccountClaimsPrincipalFactory<RemoteUserAccount> accountClaimsPrincipalFactory) :
            base(jsRuntime, options, navigation, accountClaimsPrincipalFactory) {
            _user = user;
            _httpClient = clientFactory.CreateClient();
        }


        public override async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context) {
            await JsRuntime.InvokeVoidAsync("deleteCookie", "claims", "/");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(await GetAuthenticatedUser())));
            return new RemoteAuthenticationResult<RemoteAuthenticationState>() {
                State = new RemoteAuthenticationState(), Status = RemoteAuthenticationStatus.Success
            };
        }


        public override async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context){
            
            var responseMessage = await _httpClient.PostAsJsonAsync("Authentication/Authenticate",_user);
            if (!responseMessage.IsSuccessStatusCode) {
                throw new Exception($"Authentication error({responseMessage.StatusCode}): {await responseMessage.Content.ReadAsStringAsync()}");
            }

            

            var claimsList = new List<Claim> { new(ClaimTypes.Name,_user.Username!), new("access_token", "token") };
            await JsRuntime.InvokeAsync<string>("setCookie", "claims", Uri.EscapeDataString(JsonSerializer.Serialize(claimsList)),
                DateTime.UtcNow.AddDays(1).ToString("R", CultureInfo.InvariantCulture), "/", "strict", true);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(claimsList, nameof(WebAPIAuthenticationService))))));
            return new RemoteAuthenticationResult<RemoteAuthenticationState>() {
                State = new RemoteAuthenticationState() { ReturnUrl = "/" },
                Status = RemoteAuthenticationStatus.Success
            };
        }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            var user = await GetAuthenticatedUser();
            return await Task.FromResult(new AuthenticationState(user));
        }

        protected override async ValueTask<ClaimsPrincipal> GetAuthenticatedUser() {
            var cookie = await JsRuntime.InvokeAsync<string>("getCookie","claims");
            return string.IsNullOrEmpty(cookie) ? new ClaimsPrincipal(new ClaimsIdentity())
                : new ClaimsPrincipal(new ClaimsIdentity(
                    JsonSerializer.Deserialize<Claim[]>(Uri.UnescapeDataString(cookie)),
                    nameof(WebAPIAuthenticationService)));
        }

    }
} 