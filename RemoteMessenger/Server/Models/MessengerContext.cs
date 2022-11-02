using Microsoft.EntityFrameworkCore;

namespace RemoteMessenger.Server.Models;

public class MessengerContext : DbContext
{
    public MessengerContext(DbContextOptions<MessengerContext> options) : base(options) {}

    public DbSet<User> Users;
    public DbSet<RegisterCode> RegisterCodes;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<RegisterCode>().ToTable("Register Code");
    }
}