using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RemoteServer.Models;

public class ChatMessage
{
    [Key]
    [Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ChatMessageId { get; set; }

    [JsonIgnore] public User? FromUser { get; set; }

    [JsonIgnore] public User? ToUser { get; set; }

    public required string Body { get; set; }

    public required DateTime SentOn { get; set; }
}