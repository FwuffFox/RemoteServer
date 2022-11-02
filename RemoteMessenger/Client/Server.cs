using System.Net.Http.Headers;

namespace RemoteMessenger.Client;

public static class Server
{
    private const string ServerBaseUrl = "https://localhost:7219";
    public static HttpClient? ServerConnection;
    public static void Initialize()
    {
        var res = new HttpClient();
        res.BaseAddress = new Uri(ServerBaseUrl);
        res.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        res.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
    }
}