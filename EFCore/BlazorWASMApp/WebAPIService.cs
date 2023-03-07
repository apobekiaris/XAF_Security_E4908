using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using BlazorWASMApp.Client.Pages;

namespace BlazorWASMApp.Client;




public class WebAPIService  {
    private readonly HttpClient _httpClient;

    private readonly string _apiUrl = "https://localhost:5001/api/";
    private readonly string _postEndPointUrl;
    private const string ApplicationJson = "application/json";

    public WebAPIService(HttpClient httpClient) {
        _httpClient = httpClient;
        _postEndPointUrl = _apiUrl + "odata/" + nameof(Post);
    }

    public async Task<bool> UserCanCreatePostAsync()
        => (bool)JsonNode.Parse(await _httpClient.GetStringAsync($"{_apiUrl}CustomEndpoint/CanCreate?typename=Post"))!;

    public async Task<byte[]> GetAuthorPhotoAsync(int postId)
        => await _httpClient.GetByteArrayAsync($"{_apiUrl}CustomEndPoint/AuthorPhoto/{postId}");

    public async Task ArchivePostAsync(Post post) {
        var httpResponseMessage = await _httpClient.PostAsync($"{_apiUrl}CustomEndPoint/Archive", new StringContent(JsonSerializer.Serialize(post), Encoding.UTF8, ApplicationJson));
        if (httpResponseMessage.IsSuccessStatusCode) {
            // await Shell.Current.DisplayAlert("Success", "This post is saved to disk", "Ok");
        }
        else {
            // await Shell.Current.DisplayAlert("Error", await httpResponseMessage.Content.ReadAsStringAsync(), "Ok");
        }
    }

    public async Task ShapeIt() {
        throw new NotImplementedException();

//         var bytes = await _httpClient.GetByteArrayAsync($"{_apiUrl}report/DownloadByName(Post Report)");
// #if ANDROID
// 		var fileName = $"{FileSystem.Current.AppDataDirectory}/Report.pdf";
// 		await File.WriteAllBytesAsync(fileName, bytes);
// 		var intent = new Android.Content.Intent(Android.Content.Intent.ActionView);
// 		intent.SetDataAndType(AndroidX.Core.Content.FileProvider.GetUriForFile(Android.App.Application.Context,
// 			$"{Android.App.Application.Context.ApplicationContext?.PackageName}.provider",new Java.IO.File(fileName)),"application/pdf");
// 		intent.SetFlags(Android.Content.ActivityFlags.ClearWhenTaskReset | Android.Content.ActivityFlags.NewTask | Android.Content.ActivityFlags.GrantReadUriPermission);
// 		Android.App.Application.Context.ApplicationContext?.StartActivity(intent);
// #else
//         var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
//         var fileName = $"{path}/Report.pdf";
//         await File.WriteAllBytesAsync(fileName, bytes);
//         var filePath = Path.Combine(path, "Report.pdf");
//         var viewer = UIKit.UIDocumentInteractionController.FromUrl(Foundation.NSUrl.FromFilename(filePath));
//         viewer.PresentOpenInMenu(new System.Drawing.RectangleF(0, -260, 320, 320), Platform.GetCurrentUIViewController()!.View!, true);
// #endif
    }

    public async Task<bool> AddItemAsync(Post post) {
        var httpResponseMessage = await _httpClient.PostAsync(_postEndPointUrl,
            new StringContent(JsonSerializer.Serialize(post), Encoding.UTF8, ApplicationJson));
        if (!httpResponseMessage.IsSuccessStatusCode) {
            throw new HttpRequestException(await httpResponseMessage.Content.ReadAsStringAsync());
            // await Shell.Current.DisplayAlert("Error", await httpResponseMessage.Content.ReadAsStringAsync(), "OK");
        }
        return httpResponseMessage.IsSuccessStatusCode;
    }

    public async Task<Post> GetItemAsync(string id)
        => (await RequestItemsAsync($"?$filter={nameof(Post.PostId)} eq {id}")).FirstOrDefault()!;

    public async Task<IEnumerable<Post>> GetItemsAsync(bool forceRefresh = false)
        => await RequestItemsAsync();


    private async Task<IEnumerable<Post>> RequestItemsAsync(string query = null!)
        => JsonNode.Parse(await _httpClient.GetStringAsync($"{_postEndPointUrl}{query}"))!["value"].Deserialize<IEnumerable<Post>>()!;

    public event Action? OnAuthenticationStateChanged;
    
    public async Task<string> Authenticate(Login.UserDataBase user) {
        var tokenResponse = await RequestTokenAsync(user);
        var reposeContent = await tokenResponse.Content.ReadAsStringAsync();
        if (tokenResponse.IsSuccessStatusCode) {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", reposeContent);
            OnAuthenticationStateChanged?.Invoke();
            return string.Empty;
        }
        return reposeContent;
    }

    private async Task<HttpResponseMessage> RequestTokenAsync(Login.UserDataBase user) {
        try {
            return await _httpClient.PostAsJsonAsync(new Uri($"{_apiUrl}Authentication/Authenticate"),user);
	    }
	    catch (Exception) {
		    return new HttpResponseMessage(System.Net.HttpStatusCode.BadGateway) { Content = new StringContent("An error occurred during the processing of the request. Please consult the demo's ReadMe file (Step 1,10) to discover potential causes and find solutions.") };
	    }
    }

    
    public void Logout() {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        OnAuthenticationStateChanged?.Invoke();
    }
}

