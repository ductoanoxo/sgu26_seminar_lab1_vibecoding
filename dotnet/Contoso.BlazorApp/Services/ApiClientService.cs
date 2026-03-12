using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;

namespace Contoso.BlazorApp.Services;

public class ApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public ApiClientService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("Content-Type", "application/json");

        try
        {
            var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "user");
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonSerializer.Deserialize<Models.User>(userJson, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                if (user?.UserId != null)
                {
                    request.Headers.Add("X-User-ID", user.UserId.ToString());
                }
                if (!string.IsNullOrEmpty(user?.Username))
                {
                    request.Headers.Add("x-username", Uri.EscapeDataString(user.Username));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding auth headers: {ex.Message}");
        }

        return request;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        var request = await CreateRequestAsync(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string url, object? data = null)
    {
        var request = await CreateRequestAsync(HttpMethod.Post, url);
        if (data != null)
        {
            request.Content = JsonContent.Create(data);
        }
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PatchAsync<T>(string url, object? data = null)
    {
        var request = await CreateRequestAsync(HttpMethod.Patch, url);
        if (data != null)
        {
            request.Content = JsonContent.Create(data);
        }
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task DeleteAsync(string url)
    {
        var request = await CreateRequestAsync(HttpMethod.Delete, url);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T?> DeleteAsync<T>(string url)
    {
        var request = await CreateRequestAsync(HttpMethod.Delete, url);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
}
