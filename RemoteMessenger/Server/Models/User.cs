using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Server.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; init; }

    public string Username { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
    
    public string JobTitle { get; set; } = string.Empty;
    
    public byte[] PasswordHash { get; set; }
    
    public byte[] PasswordSalt { get; set; }
}