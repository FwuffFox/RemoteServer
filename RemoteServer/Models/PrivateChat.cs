using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RemoteServer.Models;

public class PrivateChat
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
     
    public User[] Users { get; set; } = Array.Empty<User>();
    
    
    [JsonIgnore]
    public List<PrivateMessage> Messages = new();

    public async Task<bool> IsUserInChatAsync(string username)
    {
        return await Task.Run(() => Users.Any(user => user.Username == username));
    }

    public bool IsUserInChat(string username)
    {
        return Users.Any(user => user.Username == username);
    }
}