using System.ComponentModel.DataAnnotations;

namespace RemoteMessenger.Shared;

public class RegistrationFormDto : UserBaseFormDto
{
    private const string EmailValidationRegularExpression = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

    [Required(ErrorMessage = "Пожалуйста укажите полное имя.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пожалуйста укажите должность.")]
    public string JobTitle { get; set; } = string.Empty;
}