using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BlazorApp7.Services{
    public class WebAPIAuthenticationStateService:RemoteAuthenticationService<WebAPIAuthenticationState,RemoteUserAccount,RemoteAuthenticationUserOptions> {
        private readonly IUserDataStorage _storage;
        public class Claim:System.Security.Claims.Claim {
            public Claim(string type, string value) : base(type, value) {
            }
        }
        public WebAPIAuthenticationStateService(IUserDataStorage storage,IJSRuntime jsRuntime,IOptionsSnapshot<RemoteAuthenticationOptions<RemoteAuthenticationUserOptions>> options,
            NavigationManager navigation, AccountClaimsPrincipalFactory<RemoteUserAccount> accountClaimsPrincipalFactory) :
            base(jsRuntime, options, navigation, accountClaimsPrincipalFactory) 
            => _storage = storage;

        public override async Task<RemoteAuthenticationResult<WebAPIAuthenticationState>> SignOutAsync(RemoteAuthenticationContext<WebAPIAuthenticationState> context){
            await _storage.RemoveAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(await GetAuthenticatedUser())));
            return new RemoteAuthenticationResult<WebAPIAuthenticationState>() {
                State = new WebAPIAuthenticationState(), Status = RemoteAuthenticationStatus.Success
            };
        }

        public override async ValueTask<AccessTokenResult> RequestAccessToken() 
            => new(AccessTokenResultStatus.Success, new AccessToken
                    { Value = (await GetAuthenticatedUser()).FindFirst(claim => claim.Type == "access_token")?.Value }, "/");

        public override async Task<RemoteAuthenticationResult<WebAPIAuthenticationState>> SignInAsync(RemoteAuthenticationContext<WebAPIAuthenticationState> context){
            var claimsList = new List<Claim> { new(ClaimTypes.Name,context.State.UserName!), new("access_token", context.State.Token!) };
            await _storage.SaveAsync(JsonSerializer.Serialize(claimsList));
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(claimsList, nameof(WebAPIAuthenticationStateService))))));
            return new RemoteAuthenticationResult<WebAPIAuthenticationState>{
                State = context.State, Status = RemoteAuthenticationStatus.Success
            };
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
            => await Task.FromResult(new AuthenticationState(await GetAuthenticatedUser()));

        protected override async ValueTask<ClaimsPrincipal> GetAuthenticatedUser() {
            var cookie = await _storage.GetAsync();
            return string.IsNullOrEmpty(cookie) ? new ClaimsPrincipal(new ClaimsIdentity())
                : new ClaimsPrincipal(new ClaimsIdentity(JsonSerializer.Deserialize<Claim[]>(cookie),
                    nameof(WebAPIAuthenticationStateService)));
        }

    }
} 