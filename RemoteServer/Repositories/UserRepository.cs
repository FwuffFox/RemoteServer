using RemoteServer.Models.DbContexts;

namespace RemoteServer.Repositories;

public class UserRepository
{
    private readonly MessengerContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(MessengerContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public MessengerContext Context
    {
        get => _context;
        private set => value = _context;
    }

    public async Task<User?> GetUserAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<User?> GetUserAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.UserId == id);
    }

    public async Task<IQueryable<User>> GetAllUsersAsync()
    {
        return await Task.Run(() => _context.Users.AsQueryable());
    }

    public async Task<bool> IsUsernameTaken(string username)
    {
        return await _context.Users.AnyAsync(user => user.Username == username);
    }

    public async Task CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"{user.Username} was registered as {user.Role}");
    }
}