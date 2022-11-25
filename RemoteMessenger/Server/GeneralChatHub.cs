using Microsoft.AspNetCore.SignalR;

namespace RemoteMessenger.Server;

[Authorize]
public class GeneralChatHub : Hub
{
    public const string HubUrl = "/general_chat";
    private readonly ILogger<GeneralChatHub> _logger;

    public GeneralChatHub(ILogger<GeneralChatHub> logger)
    {
        _logger = logger;
    }

    public async Task Broadcast(string username, string message)
    {
        await Clients.All.SendAsync("Broadcast", username, message);
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation($"{Context.ConnectionId} connected");
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? e)
    {
        _logger.LogInformation($"Disconnected {e?.Message} {Context.ConnectionId}");
        await base.OnDisconnectedAsync(e);
    }
}