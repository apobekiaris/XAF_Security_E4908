using System.Net.Http.Headers;

namespace BlazorApp7.Services {
    public class AuthorizationHandler : DelegatingHandler {
        private readonly WebAPIAuthenticationStateService _authenticationStateService;

        public AuthorizationHandler(WebAPIAuthenticationStateService authenticationStateService) 
            => _authenticationStateService = authenticationStateService;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var tokenResult = await _authenticationStateService.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token)){
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            }
            return await base.SendAsync(request, cancellationToken);
        }                           
    }
}
