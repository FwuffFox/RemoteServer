using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using RemoteMessenger.Server.Services;
using RemoteMessenger.Shared.Models;

namespace RemoteMessenger.Server.Hubs;

// TODO: Fix Authorization
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    public async Task SendMessage(string message, string username)
    {
        var user = await _userService.GetUserAsync(username);
        if (user is null) return; 
        var sentMessage = new PublicMessage
        {
            Sender = user,
            Body = message,
            SendTime = DateTime.UtcNow,
        };
        await _context.PublicMessages.AddAsync(sentMessage);
        await _context.SaveChangesAsync();
        await Clients.All.SendAsync("ReceiveMessage", sentMessage);
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