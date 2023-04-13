using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using RemoteServer.Models.Shared;

namespace RemoteServer.Models;

public sealed class User
{
    private string _role = string.Empty;

    private string _username = string.Empty;

    [Key] [Column(Order = 0)] public int UserId { get; init; }

    public required string Username
    {
        get => _username;
        set
        {
            if (value[0] != '@') value = value.Insert(0, "@");
            _username = value.ToLower();
        }
    }

    public required string FullName { get; set; } = string.Empty;
    public required string JobTitle { get; set; } = string.Empty;

    public string Role
    {
        get => _role;
        set => _role = value is Roles.Admin ? value : Roles.User;
    }

    [JsonIgnore] public List<PublicMessage> PublicMessages { get; set; } = new();


    [JsonIgnore] // Secret data
    public byte[] PasswordHash { get; private set; } = Array.Empty<byte>();

    [JsonIgnore] // Secret data
    public byte[] PasswordSalt { get; private set; } = Array.Empty<byte>();

    public async Task<bool> IsPasswordValidAsync(string password)
    {
        using var hmac = new HMACSHA512(PasswordSalt);
        var passStream = new MemoryStream(Encoding.UTF8.GetBytes(password));
        var computeHash = await hmac.ComputeHashAsync(passStream);
        return computeHash.SequenceEqual(PasswordHash);
    }

    /// <summary>
    ///     Creates a new Salt and Hash for <see cref="User" />.
    /// </summary>
    public async Task SetPassword(string newPassword)
    {
        using var hmac = new HMACSHA512();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(newPassword));
        PasswordSalt = hmac.Key;
        PasswordHash = await hmac.ComputeHashAsync(stream);
    }
}

internal class UserEqualityComparer : IEqualityComparer<User>
{
    public bool Equals(User? x, User? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Username == y.Username;
    }

    public int GetHashCode(User obj)
    {
        return obj.Username.GetHashCode();
    }
}