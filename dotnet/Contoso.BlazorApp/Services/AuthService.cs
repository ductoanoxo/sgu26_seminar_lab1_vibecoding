using Microsoft.JSInterop;
using System.Text.Json;
using Contoso.BlazorApp.Models;

namespace Contoso.BlazorApp.Services;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;
    private User? _currentUser;

    public event Action? OnChange;

    public AuthService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public User? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;
    public bool IsLoading { get; private set; } = true;

    public async Task InitializeAsync()
    {
        try
        {
            var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "user");
            if (!string.IsNullOrEmpty(userJson))
            {
                _currentUser = JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task<User?> LoginAsync(string username)
    {
        try
        {
            IsLoading = true;
            var userData = new User { Username = username.Trim() };
            _currentUser = userData;
            
            var userJson = JsonSerializer.Serialize(userData);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user", userJson);
            
            return userData;
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user");
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
