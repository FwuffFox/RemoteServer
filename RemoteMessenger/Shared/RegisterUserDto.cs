namespace RemoteMessenger.Shared;

public class RegisterUserDto
{
    public string RegistrationCode { get; set; } = string.Empty;
    
    private string _username = string.Empty;

    public string Username
    {
        get => _username;
        set => _username = value.ToLower();
    }
    
    public string Password { get; set; } = string.Empty;
}