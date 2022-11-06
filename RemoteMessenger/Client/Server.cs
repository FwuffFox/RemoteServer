using System.Net.Http.Headers;

namespace RemoteMessenger.Client;

public class Server
{
    private const string ServerBaseUrl = "https://localhost:5001";
    public readonly HttpClient? ServerClient;

    public Server()
    {
        ServerClient ??= new HttpClient
        {
            BaseAddress = new Uri(ServerBaseUrl)
        };
    }

    public void AddJwtToken(string token) =>
        ServerClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
}