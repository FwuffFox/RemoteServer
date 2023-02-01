namespace RemoteServer.Models.DbContexts;

public sealed class MessengerContext : DbContext
{
    public MessengerContext(DbContextOptions<MessengerContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; } = null!;
    
    public DbSet<PrivateMessage> PrivateMessages { get; set; } = null!;
    public DbSet<PrivateChat> PrivateChats { get; set; } = null!;
    public DbSet<PublicMessage> PublicMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<PrivateMessage>().ToTable("Private Message");
        modelBuilder.Entity<PrivateChat>().ToTable("Private Chat");
        modelBuilder.Entity<PublicMessage>().ToTable("Public Message");
    }
}