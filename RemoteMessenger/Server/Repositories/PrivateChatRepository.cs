namespace RemoteMessenger.Server.Repositories;

public class PrivateChatRepository
{
    private readonly MessengerContext _context;

    public PrivateChatRepository(MessengerContext context)
    {
        _context = context;
    }

    public async Task<PrivateChat?> GetPrivateChat(string firstUsername, string secondUsername)
    {
        return await _context.PrivateChats.FirstOrDefaultAsync(chat =>
            chat.IsUserInChat(firstUsername) && chat.IsUserInChat(secondUsername));
    }

    public async Task CreateNewPrivateChat(PrivateChat chat)
    {
        await _context.PrivateChats.AddAsync(chat);
        await _context.SaveChangesAsync();
    }
}