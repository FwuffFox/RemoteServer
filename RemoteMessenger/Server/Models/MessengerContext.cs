using RemoteMessenger.Server.Models.PrivateChat;
using RemoteMessenger.Shared.Models;

namespace RemoteMessenger.Server.Models;

public sealed class MessengerContext : DbContext
{
    public MessengerContext(DbContextOptions<MessengerContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RegistrationCode> RegistrationCodes { get; set; } = null!;
    public DbSet<PrivateMessage> PrivateMessages { get; set; } = null!;
    public DbSet<PrivateChat.PrivateChat> PrivateChats { get; set; } = null!;
    public DbSet<PublicMessage> PublicMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<RegistrationCode>().ToTable("Registration Code");
        modelBuilder.Entity<PrivateMessage>().ToTable("Private Message");
        modelBuilder.Entity<PrivateChat.PrivateChat>().ToTable("Private Chat");
        modelBuilder.Entity<PublicMessage>().ToTable("Public Message");
    }
}