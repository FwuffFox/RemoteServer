using Microsoft.AspNetCore.SignalR;
using RemoteServer.Models.DbContexts;
using RemoteServer.Util;

namespace RemoteServer.Hubs;

// [Authorize]
public class ChatHub : Hub
{
    public const string HubUrl = "/hubs/private_chat";
    private readonly ChatMessagesRepository _chatMessagesRepository;
    private readonly MessengerContext _context;
    private readonly UserRepository _userRepository;

    public ChatHub(MessengerContext context, UserRepository userRepository,
        ChatMessagesRepository chatMessagesRepository)
    {
        _context = context;
        _userRepository = userRepository;
        _chatMessagesRepository = chatMessagesRepository;
    }

    private string IdentityName => Context.User?.GetUniqueName()!;

    public override async Task OnConnectedAsync()
    {
        var me = await _userRepository.GetUserAsync(IdentityName);
        var myChatInfos = await _chatMessagesRepository.GetAllUserChats(me!);
        await Clients.Caller.SendAsync("OnConnected", myChatInfos);
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string message, string receiverName)
    {
        var me = await _userRepository.GetUserAsync(IdentityName);
        var receiver = await _userRepository.GetUserAsync(receiverName);
        if (me is null || receiver is null) return;

        var chatMessage = new ChatMessage
        {
            FromUser = me,
            ToUser = receiver,
            Body = message,
            SentOn = DateTime.Now
        };
        await _chatMessagesRepository.SaveChatMessageAsync(chatMessage);

        var messageWithSender = new MessageWithSender
        {
            Sender = me,
            Message = chatMessage
        };

        await Clients.User(receiverName).SendAsync("OnGetMessage", messageWithSender);
    }

    public async Task<ChatInfo?> GetChatInfo(string otherUserName)
    {
        var me = await _userRepository.GetUserAsync(IdentityName);
        var otherUser = await _userRepository.GetUserAsync(otherUserName);
        if (me is null || otherUser is null) return null;
        var chatInfo = await _chatMessagesRepository.GetChatInfoAsync(me, otherUser);
        return chatInfo;
    }

    private struct MessageWithSender
    {
        public User Sender { get; set; }
        public ChatMessage Message { get; set; }
    }
}