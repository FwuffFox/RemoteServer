using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Server.Models.PrivateChat;

public class PrivateChat
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public List<PrivateMessage> Messages = new();
}