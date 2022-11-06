using System.ComponentModel.DataAnnotations;

namespace RemoteMessenger.Shared;

public class RegisterUserDto : UserBaseDto
{
    [Required(ErrorMessage = "Требуется регистрационный код")]
    [DataType(DataType.Password)]
    public string RegistrationCode { get; set; } = string.Empty;
    
    public string Gender { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Требуется полное имя пользователя.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется должность.")]
    public string JobTitle { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Требуется дата рождения.")]
    [RegularExpression(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]|(?:Jan|Mar|May|Jul|Aug|Oct|Dec)))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2]|(?:Jan|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)(?:0?2|(?:Feb))\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9]|(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep))|(?:1[0-2]|(?:Oct|Nov|Dec)))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$")]
    public string DateOfBirth { get; set; } = string.Empty;
}