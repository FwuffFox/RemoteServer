namespace RemoteMessenger.Shared;

public class RegistrationCodeDto
{
    public string Code { get; set; } = string.Empty;

    private string _role = string.Empty;

    public string Role
    {
        get => _role;
        set => _role = value is Roles.Admin ? value : Roles.User;
    }
}