namespace RemoteMessenger.Shared;

/// <summary>
/// Server must accept LoginUserDto as a parameter in it's API.
/// Client should send LoginUserDto as a request body.
/// </summary>
public class LoginUserDto
{
    private string _username = string.Empty;

    public string Username
    {
        get => _username;
        set => _username = value.ToLower();
    }
    
    public string Password { get; set; } = string.Empty;
}