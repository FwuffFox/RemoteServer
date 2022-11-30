using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RemoteMessenger.Shared.Models;

namespace RemoteMessenger.Server.Models.PrivateChat;

public class PrivateMessage
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public required PrivateChat Chat { get; set; }

    public required User Sender { get; set; }

    public required User Receiver { get; set; }

    public required string Body { get; set; }

    public required DateTime SendTime { get; set; }
}