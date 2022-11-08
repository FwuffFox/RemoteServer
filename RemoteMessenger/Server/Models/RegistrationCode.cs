using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteMessenger.Server.Models;

/// <summary>
///     Codes which are used to register unique users.
/// </summary>
public class RegistrationCode
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; init; }

    public string Code { get; set; } = string.Empty;

    private string _role = string.Empty;

    public string Role
    {
        get => _role;
        set => _role = value is Roles.Admin ? value : Roles.User;
    }
}
