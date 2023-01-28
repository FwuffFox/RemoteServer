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
        return await _context.PrivateChats
            .Include(x => x.Users)
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(chat =>
            chat.IsUserInChat(firstUsername) && chat.IsUserInChat(secondUsername));
    }

    public IQueryable<PrivateChat> GetUserPrivateChats(string username)
    {
        return _context.PrivateChats
            .Include(x => x.Users)
            .Include(x => x.Messages)
            .Where(chat => chat.IsUserInChat(username));
    }

    public async Task CreateNewPrivateChat(PrivateChat chat)
    {
        await _context.PrivateChats.AddAsync(chat);
        await _context.SaveChangesAsync();
    }
}