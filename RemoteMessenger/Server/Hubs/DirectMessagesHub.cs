using System.IdentityModel.Tokens.Jwt;
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
        get => Context.User?.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value!;
    }
    
    public async Task SendPrivateMessage(string receiver, string message)
    {
        var receiverUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == receiver);
        if (receiverUser is null) return;
        await Clients.User(receiver).SendAsync("GetMessage");
    }
    
    
}