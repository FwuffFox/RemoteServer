using System.ComponentModel.DataAnnotations.Schema;
using RemoteMessenger.Shared;

namespace RemoteMessenger.Server.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; init; }

    private string _username = string.Empty;

    public string Username
    {
        get => _username;
        set
        {
            if (value[0] != '@') value = value.Insert(0, "@");
            _username = value.ToLower();
        }
    }

    public string FullName { get; set; } = string.Empty;
    
    public string JobTitle { get; set; } = string.Empty;
    
    public byte[] PasswordHash { get; set; }
    
    public byte[] PasswordSalt { get; set; }
}