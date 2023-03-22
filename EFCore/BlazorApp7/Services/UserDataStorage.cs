using System.Globalization;
using Microsoft.JSInterop;

namespace BlazorApp7.Services{
    public interface IUserDataStorage{
        Task RemoveAsync();
        Task SaveAsync(string data);
        Task<string?> GetAsync();
    }

    public class UserDataStorage:IUserDataStorage{
        private readonly IJSRuntime _jsRuntime;
        private const string CookieName = "dts";

        public UserDataStorage(IJSRuntime jsRuntime) 
            => _jsRuntime = jsRuntime;

        public async Task RemoveAsync() 
            => await _jsRuntime.InvokeVoidAsync("deleteCookie", CookieName, "/");

        public async Task SaveAsync(string data) 
            => await _jsRuntime.InvokeAsync<string>("setCookie", CookieName, Uri.EscapeDataString(data),
                DateTime.UtcNow.AddDays(1).ToString("R", CultureInfo.InvariantCulture), "/", "strict", true);

        public async Task<string?> GetAsync(){
            var data = await _jsRuntime.InvokeAsync<string>("getCookie", CookieName);
            return string.IsNullOrEmpty(data) ? data : Uri.UnescapeDataString(data);
        }
    }
}