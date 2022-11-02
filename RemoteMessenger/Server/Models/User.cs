using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Server.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; init; }
    
    public string Username { get; set; }
    
    /// <summary>
    /// Used to encrypt things that are sent to a user from server. Stored in Base64
    /// </summary>
    public string PublicRsaKey { get; set; }
    
    /// <summary>
    /// Password hashed by SHA256 stored in Base64.
    /// </summary>
    public string HashedPassword { get; set; }
}