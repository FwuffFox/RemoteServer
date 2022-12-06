using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace RemoteMessenger.Shared.Models;

public sealed class User
{
    [Key, Column(Order = 0)]
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
            _gender = value is Genders.Male or Genders.Female ? value : "Undefined";
        }
    }
    
    public string DateOfBirth { get; set; } = string.Empty;
    
    [JsonIgnore] // Secret data
    public byte[] PasswordHash { get; private set; } = Array.Empty<byte>();

    [JsonIgnore] // Secret data
    public byte[] PasswordSalt { get; private set; } = Array.Empty<byte>();

    public async Task<bool> IsPasswordValidAsync(string password)
    {
        using var hmac = new HMACSHA512(key: PasswordSalt);
        var passStream = new MemoryStream(Encoding.UTF8.GetBytes(password));
        var computeHash = await hmac.ComputeHashAsync(passStream);
        return computeHash.SequenceEqual(PasswordHash);
    }

    /// <summary>
    /// Creates a new Salt and Hash for <c>User</c>.
    /// </summary>
    public async Task SetPassword(string newPassword)
    {
        using var hmac = new HMACSHA512();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(newPassword));
        PasswordSalt = hmac.Key;
        PasswordHash = await hmac.ComputeHashAsync(stream);
    }
}