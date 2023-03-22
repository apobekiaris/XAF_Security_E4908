using System.Net;
using System.Net.Http.Headers;

namespace BlazorApp3.Client.Services
{
    public class AuthorizedHandler : DelegatingHandler
    {
        private readonly WebAPIAuthenticationService _authenticationStateProvider;

        public AuthorizedHandler(WebAPIAuthenticationService authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var tokenResult = await _authenticationStateProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token)){
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            }
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            HttpResponseMessage responseMessage;
            if (!authState.User.Identity!.IsAuthenticated)
            {
                // if user is not authenticated, immediately set response status to 401 Unauthorized
                responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            else
            {
                responseMessage = await base.SendAsync(request, cancellationToken);
            }

            if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                // if server returned 401 Unauthorized, redirect to login page
                await _authenticationStateProvider.SignInAsync(default!);
            }

            return responseMessage;
        }
    }
}
