using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace RemoteMessenger.Server.Hubs;

[Authorize]
public class DirectMessagesHub : Hub
{
    public const string HubUrl = "/private_chat";
    private readonly MessengerContext _context;
    public DirectMessagesHub(MessengerContext context)
    {
        _context = context;
    }
    
    private string IdentityName
    {
        get => Context.User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
    
    public async Task Broadcast(string receiver, string message)
    {
        var receiverUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == receiver);
        if (receiverUser is null) return;
        await Clients.All.SendAsync("Broadcast", IdentityName, message);
        //await Clients.User(IdentityName).SendAsync("ReceiveMessage",IdentityName, message);
    }
}