using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace RemoteMessenger.Client.Services;

public class JwtManager
{
    private readonly HttpClient _client;
    private readonly ProtectedLocalStorage _storage;
    private readonly ILogger<JwtManager> _logger;

    public JwtManager(HttpClient client, ProtectedLocalStorage storage, ILogger<JwtManager> logger)
    {
        _client = client;
        _storage = storage;
        _logger = logger;
    }

    public async Task<bool> ValidateJwt(string jwt)
    {
        var response = await _client.GetStringAsync($"/validate_jwt?jwt={jwt}");
        return response == "true";
    }

    public async Task<string?> LoadJwt()
    {
        var jwt = await _storage.GetAsync<string>("Jwt");
        return jwt.Value;
    }

    public async Task SaveJwt(string jwt)
    {
        await _storage.SetAsync("Jwt", jwt);
    }
}