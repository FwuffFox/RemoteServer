using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Shared.Models;

public class PrivateChat
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
     
    public User[] Users { get; set; } = Array.Empty<User>();
    
    public List<PrivateMessage> Messages = new();

    public async Task<bool> IsUserInChat(string username)
    {
        return await Task.Run(() => Users.Any(user => user.Username == username));
    }
}