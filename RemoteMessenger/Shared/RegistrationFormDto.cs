using System.ComponentModel.DataAnnotations;

namespace RemoteMessenger.Shared;

public class RegistrationFormDto : UserBaseFormDto
{
    [Required(ErrorMessage = "Требуется полное имя пользователя.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется должность.")]
    public string JobTitle { get; set; } = string.Empty;
}