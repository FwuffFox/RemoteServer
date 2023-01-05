using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Shared.Models;

public class PrivateChat
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public required User Sender { get; set; }
    
    public required User Receiver { get; set; }

    public async Task<bool> IsUserInChat(string username)
    {
        return await Task.Run(() => Sender.Username == username || Receiver.Username == username);
    }

    public List<PrivateMessage> Messages = new();
}