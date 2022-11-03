using Microsoft.EntityFrameworkCore;

namespace RemoteMessenger.Server.Models;

public sealed class MessengerContext : DbContext
{
    public MessengerContext(DbContextOptions<MessengerContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RegisterCode> RegisterCodes { get; set; } = null!;

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<RegisterCode>().ToTable("Register Code");
    }
}