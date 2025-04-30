using Microsoft.Maui.Storage;

namespace FreyaDev.Services;

public class UserSessionService
{
    private const string TokenKey = "auth_token";
    private const string UserKey = "current_user";


    public User? GetCurrentUser()
    {
        var userJson = Preferences.Get(UserKey, null);
        if (string.IsNullOrEmpty(userJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<User>(userJson);
        }
        catch
        {
            return null;
        }
    }
    public int GetCurrentRole()
    {
        return GetCurrentUser()?.RoleId ?? 0;
    }

    public string? GetCurrentUsername()
    {
        return GetCurrentUser()?.Username;
    }

    public void SetCurrentUser(User user)
    {
        var json = JsonSerializer.Serialize(user);
        Preferences.Set(UserKey, json);
    }

    public async Task SetAuthTokenAsync(string token)
    {
        await SecureStorage.SetAsync(TokenKey, token);
    }

    public async Task<string?> GetAuthTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public void Logout()
    {
        Preferences.Remove(UserKey);
        SecureStorage.Remove(TokenKey);
        Preferences.Set("IsLoggedIn", false);
    }
}
