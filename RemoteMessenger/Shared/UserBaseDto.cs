namespace RemoteMessenger.Shared;

public class UserBaseDto
{
    private string _username = string.Empty;

    public string Username
    {
        get => _username;
        set
        {
            if (value[0] != '@') value = value.Insert(0, "@");
            _username = value.ToLower();
        }
    }
    
    public string Password { get; set; } = string.Empty;
}