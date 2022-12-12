using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Shared.Models;

public class PrivateChat
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public required User FirstUser { get; set; }
    
    public required User SecondUser { get; set; }

    public async Task<bool> IsUserInChat(string username)
    {
        return await Task.Run(() => FirstUser.Username == username || SecondUser.Username == username);
    }

    public List<PrivateMessage> Messages = new();
}