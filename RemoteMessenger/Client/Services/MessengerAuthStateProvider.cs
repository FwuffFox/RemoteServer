using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace RemoteMessenger.Client.Services;

public class MessengerAuthStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState AnonymousState = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private readonly JwtManager _jwtManager;

    public MessengerAuthStateProvider(JwtManager jwtManager)
    {
        _jwtManager = jwtManager;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jwtManager.LoadJwt();
        if (token is null || !await _jwtManager.ValidateJwt(token))
        {
            NotifyAuthenticationStateChanged(Task.FromResult(AnonymousState));
            return AnonymousState;
        }
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);
        
        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }
    
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
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