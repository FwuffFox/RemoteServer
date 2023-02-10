using RemoteServer.Models.DbContexts;

namespace RemoteServer.Repositories;

public class ChatMessagesRepository
{
    private readonly MessengerContext _context;
    private readonly ILogger<ChatMessagesRepository> _logger;
    private readonly UserRepository _userRepository;

    public ChatMessagesRepository(MessengerContext context, UserRepository userRepository,
        ILogger<ChatMessagesRepository> logger)
    {
        _context = context;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ChatMessage> SaveChatMessageAsync(ChatMessage message)
    {
        await _context.ChatMessages.AddAsync(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public IQueryable<ChatMessage> GetChatMessagesByUserId
        (int fromUserId, int toUserId, int lastMessageId = 0)
    {
        return _context.ChatMessages
            .Where(msg =>
                (msg.ToUser.UserId == toUserId || msg.FromUser.UserId == toUserId) &&
                (msg.ToUser.UserId == fromUserId || msg.FromUser.UserId == fromUserId));
    }

    public IQueryable<ChatMessage> GetChatMessagesByUserName
        (string fromUserName, string toUserName)
    {
        return _context.ChatMessages
            .Where(msg => msg.FromUser.Username == fromUserName && msg.ToUser.Username == toUserName);
    }

    public async Task<ChatInfo> GetChatInfoAsync(User me, User otherUser)
    {
        return new ChatInfo
        {
            OtherUser = otherUser,
            MessagesFromMe = await GetChatMessagesByUserName(me.Username, otherUser.Username)
                .ToListAsync(),
            MessagesToMe = await GetChatMessagesByUserName(otherUser.Username, me.Username)
                .ToListAsync()
        };
    }

    public IQueryable<User> GetAllUserContacts(User me)
    {
        return _context.ChatMessages
            .Where(msg => msg.FromUser == me || msg.ToUser == me)
            .OrderByDescending(message => message.SentOn)
            .Select(msg => msg.FromUser != me ? msg.FromUser! : msg.ToUser!)
            .Distinct();
    }

    public async Task<List<ChatInfo>> GetAllUserChats(User me)
    {
        var contacts = await GetAllUserContacts(me).ToListAsync();
        contacts = contacts.DistinctBy(user => user.Username).ToList();
        _logger.LogInformation($"Found contacts for {me.Username}: {contacts.Count}");
        var tasks = contacts.Select(async contact => await GetChatInfoAsync(me, contact));
        var chatInfos = await Task.WhenAll(tasks);
        return chatInfos.ToList();
    }
}