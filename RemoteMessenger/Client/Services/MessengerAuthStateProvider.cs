using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace RemoteMessenger.Client.Services;

public class MessengerAuthStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState AnonymousState = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private readonly HttpClient _client;

    public MessengerAuthStateProvider(HttpClient client)
    {
        _client = client;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJlOWM0NGIwOC1iMTdiLTQ3NWMtYTczNi05ODE4ZmNkNTQyMDYiLCJpYXQiOjE2Njg0NDg0MTksInVuaXF1ZV9uYW1lIjoiQGFuZHJldyIsIm5hbWUiOiJBbmRyZXcgRm94IiwiZ2VuZGVyIjoibWFsZSIsImJpcnRoZGF0ZSI6IjEzLjA5LjIwMDUiLCJyb2xlcyI6IkFkbWluIiwiZXhwIjoxNjY5NjU4MDE5LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo1MDAxLyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDAvIn0.TVELrS5D8M1iK8hQRto6zZWmrZZcYYrfBrr7Zn3IF_k3xiMpmwpevrZPXrawPQQDrVJK6uq_iZZ-oOgkySCiHg";
        var response = await _client.GetAsync($"/validate_jwt?jwt={token}");
        if (await response.Content.ReadAsStringAsync() == "false")
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