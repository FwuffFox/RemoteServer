using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RemoteMessenger.Shared;

namespace RemoteMessenger.Server.Models;

/// <summary>
///     Codes which are used to register unique users.
/// </summary>
public class RegistrationCode : RegistrationCodeDto
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
}

