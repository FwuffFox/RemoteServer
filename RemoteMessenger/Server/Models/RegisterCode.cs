using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Server.Models;

public class RegisterCode
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string? Code { get; set; }
}