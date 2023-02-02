using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using RemoteServer.Util;
using RemoteServer.Models;
using RemoteServer.Models.DbContexts;
using RemoteServer.Repositories;

namespace RemoteServer.Hubs;

// [Authorize]
public class ChatHub : Hub
{
    public const string HubUrl = "/hubs/private_chat";
    private readonly MessengerContext _context;
    private readonly UserRepository _userRepository;
    private readonly ChatMessagesRepository _chatMessagesRepository;
    public ChatHub(MessengerContext context, UserRepository userRepository, ChatMessagesRepository chatMessagesRepository)
    {
        _context = context;
        _userRepository = userRepository;
        _chatMessagesRepository = chatMessagesRepository;
    }
    
    private string IdentityName
    {
        get => Context.User?.GetUniqueName()!;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string message, string receiver)
    {
        
    }

    public async Task<List<ChatMessage>?> GetMessages(string chatName)
    {
        return null;
    }
}