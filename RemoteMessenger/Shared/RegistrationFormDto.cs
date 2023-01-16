using System.ComponentModel.DataAnnotations;

namespace RemoteMessenger.Shared;

public class RegistrationFormDto : UserBaseFormDto
{
    private const string EmailValidationRegularExpression = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
    
    private string _email = string.Empty;
    
    [Required(ErrorMessage = "Пожалуйста, укажите электронную почту.")] 
    [RegularExpression(EmailValidationRegularExpression,
        ErrorMessage = "Адрес электронной почты не валиден.")]
    [DataType(DataType.EmailAddress)]
    public string Email
    {
        get => _email;
        set => _email = value.ToLower();
    }
    
    [Required(ErrorMessage = "Пожалуйста укажите полное имя.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пожалуйста укажите должность.")]
    public string JobTitle { get; set; } = string.Empty;
}