using Microsoft.AspNetCore.SignalR;
using RemoteMessenger.Server.Services;
using RemoteMessenger.Shared.Models;

namespace RemoteMessenger.Server.Hubs;

[Authorize]
public class GeneralChatHub : Hub
{
    public const string HubUrl = "/general_chat";
    private readonly ILogger<GeneralChatHub> _logger;
    private readonly UserService _userService;
    private readonly MessengerContext _context;
    public GeneralChatHub(ILogger<GeneralChatHub> logger, UserService userService, MessengerContext context)
    {
        _logger = logger;
        _userService = userService;
        _context = context;
    }

    public async Task Broadcast(string username, string message)
    {
        var user = await _userService.GetUserAsync(username);
        if (user is null) return;
        var messageToAdd = new PublicMessage
        {
            Sender = user,
            Body = message,
            SendTime = DateTime.UtcNow,
        };
        await _context.PublicMessages.AddAsync(messageToAdd);
        await _context.SaveChangesAsync();
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