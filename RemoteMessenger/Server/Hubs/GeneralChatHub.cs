using Microsoft.AspNetCore.SignalR;
using RemoteMessenger.Server.Services;

namespace RemoteMessenger.Server.Hubs;

// TODO: Fix Authorization
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GeneralChatHub : Hub
{
    public const string HubUrl = "/hubs/general_chat";
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
        await Clients.All.SendAsync("OnReceiveMessage", sentMessage);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"{Context.ConnectionId} connected");
        var lastMessages = await Task.Run(() =>
            _context.PublicMessages
                .Include(m => m.Sender)
                .OrderByDescending(x => x.Id)
                .Take(100));
        await Clients.Caller.SendAsync("OnConnect", lastMessages.Reverse());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? e)
    {
        _logger.LogInformation($"Disconnected {e?.Message} {Context.ConnectionId}");
        await base.OnDisconnectedAsync(e);
    }
}