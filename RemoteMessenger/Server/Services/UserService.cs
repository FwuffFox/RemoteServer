using RemoteMessenger.Server.Models;

namespace RemoteMessenger.Server.Services;

public class UserService
{
    private readonly MessengerContext _context;

    public UserService(MessengerContext context)
    {
        _context = context;
    }

    public async Task CreateUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteUser(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}