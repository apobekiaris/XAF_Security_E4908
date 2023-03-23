using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace WebAPI.API.Security;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase {
    readonly IStandardAuthenticationService securityAuthenticationService;
    public AuthenticationController(IStandardAuthenticationService securityAuthenticationService) {
        this.securityAuthenticationService = securityAuthenticationService;
    }
    [HttpPost("LoginAsync")]
    [SwaggerOperation("Checks if the user with the specified logon parameters exists in the database. If it does, authenticates this user.", "Refer to the following help topic for more information on authentication methods in the XAF Security System: <a href='https://docs.devexpress.com/eXpressAppFramework/119064/data-security-and-safety/security-system/authentication'>Authentication</a>.")]
    public Task<ActionResult> LoginAsync(
        [FromBody]
        [SwaggerRequestBody(@"For example: <br /> { ""userName"": ""Admin"", ""password"": """" }")]
        AuthenticationStandardLogonParameters logonParameters
    ) {
        try {
            ClaimsPrincipal user = securityAuthenticationService.Authenticate(logonParameters);
            if(user != null) {
                var authProperties = new AuthenticationProperties {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                    IsPersistent = true,
                };
                return Task.FromResult<ActionResult>(new SignInResult(CookieAuthenticationDefaults.AuthenticationScheme, user, authProperties));
            }
            return Task.FromResult<ActionResult>(Unauthorized("User name or password is incorrect."));
        } catch(AuthenticationException) {
            return Task.FromResult<ActionResult>(Unauthorized("User name or password is incorrect."));
        }
    }

    [HttpPost("LogoutAsync")]
    public Task<SignOutResult> LogoutAsync() {
        return Task.FromResult(new SignOutResult(CookieAuthenticationDefaults.AuthenticationScheme));
    }
}
