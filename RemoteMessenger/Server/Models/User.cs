using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Server.Models;

public sealed class User
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
    
    private string _role = string.Empty;
    
    public string Role
    {
        get => _role;
        set => _role = value is Roles.Admin ? value : Roles.User;
    }
    
    private string _gender = string.Empty;
    
    public string Gender
    {
        get => _gender;
        set
        {
            value = value.ToLower();
            _gender = value is "male" or "female" ? value : "undefined";
        }
    }
    
    public string DateOfBirth { get; set; } = string.Empty;

    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
}