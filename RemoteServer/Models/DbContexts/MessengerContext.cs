namespace RemoteServer.Models.DbContexts;

public sealed class MessengerContext : DbContext
{
    public MessengerContext(DbContextOptions<MessengerContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
    public DbSet<PublicMessage> PublicMessages { get; set; } = null!;
}