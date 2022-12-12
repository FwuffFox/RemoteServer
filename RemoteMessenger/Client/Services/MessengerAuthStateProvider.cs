using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace RemoteMessenger.Client.Services;

public class MessengerAuthStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState AnonymousState = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private readonly JwtManager _jwtManager;
    private readonly NavigationManager _navigationManager;

    public MessengerAuthStateProvider(JwtManager jwtManager, NavigationManager navigationManager)
    {
        _jwtManager = jwtManager;
        _navigationManager = navigationManager;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jwtManager.LoadJwt();
        try
        {
            if (token is null || !await _jwtManager.ValidateJwt(token))
            {
                NotifyAuthenticationStateChanged(Task.FromResult(AnonymousState));
                return AnonymousState;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);
        
        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }

    public async void LogOut()
    {
        await _jwtManager.DeleteJwt();
        await GetAuthenticationStateAsync();
    }
    
    public string GetUniqueName(AuthenticationState state)
    {
        return state.User.FindFirst("unique_name")?.Value!;
    }
    
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}