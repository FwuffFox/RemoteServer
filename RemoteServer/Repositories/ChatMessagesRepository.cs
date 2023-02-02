using RemoteServer.Models.DbContexts;

namespace RemoteServer.Repositories;

public class ChatMessagesRepository
{
    private readonly MessengerContext _context;

    public ChatMessagesRepository(MessengerContext context)
    {
        _context = context;

    }

    public async Task<ChatMessage> SaveChatMessageAsync(ChatMessage message)
    {
        await _context.ChatMessages.AddAsync(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public IOrderedQueryable<ChatMessage> GetChatMessagesByUserId
        (int fromUserId, int toUserId, int lastMessageId = 0)
    {
        var messages = from message in _context.ChatMessages
            where (message.ToUserId == toUserId || message.FromUserId == toUserId) &&
                  (message.ToUserId == fromUserId || message.FromUserId == fromUserId)
            orderby message.SentOn descending
            select message;
        return messages;
    }

}