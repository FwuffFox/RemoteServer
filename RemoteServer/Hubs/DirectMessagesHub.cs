using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using RemoteServer.Util;
using RemoteServer.Models;
using RemoteServer.Models.DbContexts;
using RemoteServer.Repositories;

namespace RemoteServer.Hubs;

// [Authorize]
public class DirectMessagesHub : Hub
{
    public const string HubUrl = "/hubs/private_chat";
    private readonly MessengerContext _context;
    private readonly UserRepository _userRepository;
    private readonly PrivateChatRepository _privateChatRepository;
    public DirectMessagesHub(MessengerContext context, UserRepository userRepository, PrivateChatRepository privateChatRepository)
    {
        _context = context;
        _userRepository = userRepository;
        _privateChatRepository = privateChatRepository;
    }
    
    private string IdentityName
    {
        get => Context.User?.GetUniqueName()!;
    }

    public override async Task OnConnectedAsync()
    {
        var chats = await _privateChatRepository.GetUserPrivateChats(IdentityName).ToListAsync();
        await Clients.Caller.SendAsync("OnConnect", chats);
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string message, string receiver)
    {
        var receiverUser = await _userRepository.GetUserAsync(receiver);
        var senderUser = await _userRepository.GetUserAsync(IdentityName);
        if (receiverUser is null || senderUser is null) return; 
        var chat = await _privateChatRepository.GetPrivateChatAsync(receiver, IdentityName);
        if (chat is null)
        {
            chat = new PrivateChat
            {
                Users = new[] { senderUser, receiverUser },
            };
            await _privateChatRepository.CreateNewPrivateChat(chat);
            await Clients.User(receiver).SendAsync("OnNewChatCreate", chat);
        }

        var privateMessage = new PrivateMessage
        {
            Body = message,
            Sender = senderUser,
            SendTime = DateTime.UtcNow,
        };
        chat.Messages.Add(privateMessage);
        await _context.SaveChangesAsync();
        await Clients.Users(IdentityName, receiver)
            .SendAsync("OnNewMessage", privateMessage);
    }

    public async Task GetMessages(string chatName)
    {
        var targetChat = await _privateChatRepository.GetPrivateChatAsync(chatName, IdentityName);
        if (targetChat is null) return;
        await Clients.Caller.SendAsync("GetMessages", targetChat.Messages);
    }
}