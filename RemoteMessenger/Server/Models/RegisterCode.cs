using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Server.Models;

/// <summary>
///     Codes which are used to register unique users.
/// </summary>
public class RegisterCode
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public string? Code { get; set; }
}