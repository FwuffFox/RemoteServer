using System.ComponentModel.DataAnnotations;

namespace RemoteServer.Models.Shared;

public class RegistrationFormDto : UserBaseFormDto
{
    [Required(ErrorMessage = "Пожалуйста укажите полное имя.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пожалуйста укажите должность.")]
    public string JobTitle { get; set; } = string.Empty;
}