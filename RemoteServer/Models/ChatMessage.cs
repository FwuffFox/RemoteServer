using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteServer.Models;

public class ChatMessage
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ChatMessageId { get; set; }
    
    public int FromUserId { get; set; }

    public int ToUserId { get; set; }

    public required string Body { get; set; }
    
    public required DateTime SentOn { get; set; }
}