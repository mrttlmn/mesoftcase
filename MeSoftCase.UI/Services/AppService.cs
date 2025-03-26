using System.Text;
using System.Text.Json;
using MeSoftCase.UI.Interfaces;
using MeSoftCase.UI.ResponseModels;

namespace MeSoftCase.UI.Services;

public class AppService : IAppService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AppService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> CreateAccessTokenAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        var apiUrl = "http://host.docker.internal:9090/api/user/login";

        var client = _httpClientFactory.CreateClient();
        var payload = new { UserName = username, Password = password };
        var response = await client.PostAsJsonAsync(apiUrl, payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Token alırken bir hata oluştu. status: {response.StatusCode}, reason: {response.ReasonPhrase}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var token = JsonSerializer.Deserialize<JsonElement>(responseContent)
            .GetProperty("accessToken")
            .GetString();

        return token;
    }

    public async Task<bool> RegisterManagerAsync(string username, string password, string referralCode, string email,
        CancellationToken cancellationToken = default)
    {
        var apiUrl = "http://host.docker.internal:9090/api/user/register/manager";
        var client = _httpClientFactory.CreateClient();
        var payload = new { UserName = username, Password = password, Email = email, ReferalCode = referralCode };
        var response = await client.PostAsJsonAsync(apiUrl, payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RegisterUserAsync(string username, string password, string email,
        CancellationToken cancellationToken = default)
    {
        var apiUrl = "http://host.docker.internal:9090/api/user/register/user";
        var client = _httpClientFactory.CreateClient();
        var payload = new { UserName = username, Password = password, Email = email };
        var response = await client.PostAsJsonAsync(apiUrl, payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<GetUsersDataResponseModel>> GetUsersDataAsync(
        CancellationToken cancellationToken = default)
    {
        var apiUrl = "http://host.docker.internal:9090/api/user/getusers";
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(apiUrl, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<IEnumerable<GetUsersDataResponseModel>>(responseContent);
        }
        return null;
        
        // null döndürmek doğru bir yaklaşım değildir, ama Back-End üzerine kendimi kurguladığım için buralara çok özenmiyorum.
    }
}