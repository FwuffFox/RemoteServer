using RemoteMessenger.Shared.Models;

namespace RemoteMessenger.Server.Services;

public class UserService 
{
    private readonly MessengerContext _context;
    private readonly ILogger<UserService> _logger;

    public MessengerContext Context 
    {
        get => _context;
        private set => value = _context;
    }

    public UserService(MessengerContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<User?> GetUserAsync(string username)
        => await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
    
    public async Task<User?> GetUserAsync(int id)
        => await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToArrayAsync();
    }

    public async Task<bool> IsUsernameTaken(string username)
        => await _context.Users.AnyAsync(user => user.Username == username);

    public async Task CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"{user.Username} was registered as {user.Role}");
    }
}