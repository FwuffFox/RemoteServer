namespace RemoteMessenger.Shared;

public class RegisterUserDto : UserBaseDto
{
    public string RegistrationCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string JobTitle { get; set; } = string.Empty;
}