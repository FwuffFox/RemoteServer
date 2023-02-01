using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteServer.Models;

public class PublicMessage
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public required User Sender { get; set; }
    
    public required string Body { get; set; }

    public required DateTime SendTime { get; set; }
}