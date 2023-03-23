using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorApp7.Services;

public class WebAPIAuthenticationState:RemoteAuthenticationState {
    public WebAPIAuthenticationState(string userName) { //}, string token) {
        UserName = userName;
        //Token = token;
    }

    public WebAPIAuthenticationState() {
        
    }

    public string? UserName { get; set; }
    //public string? Token { get; }
}