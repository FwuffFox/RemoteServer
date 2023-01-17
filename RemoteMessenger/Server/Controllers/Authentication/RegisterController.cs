using Microsoft.AspNetCore.Mvc;
using RemoteMessenger.Server.Services;
using RemoteMessenger.Server.Util;
using RemoteMessenger.Shared.Models;

namespace RemoteMessenger.Server.Controllers.Authentication;

[ApiController]
[Route("/auth/register")]
public class RegisterController : ControllerBase
{
    private readonly UserService _userService;

    public RegisterController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Регистрация
    /// </summary>
    /// <param name="requestBody">Данные для регистрации.</param>
    /// <response code="200"> Пользователь успешно зарегистрирован. Возращает JWT
    /// токен для дальнейшей аутентификации.</response>
    /// <response code="400"> Пользователь не смог зарегистрироваться. Возращаем ошибки.</response>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     POST /auth/register
    ///     {
    ///         "username": "@user",
    ///         "password": "password",
    ///         "email": email@test.com,
    ///         "fullName": "Surname Name FatherName",
    ///         "jobTitle": "Tester"
    ///     }
    /// </remarks>
    [HttpPost(Name = "RegisterUser")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status409Conflict,
        Type = typeof(Dictionary<string, string[]>))]
    public async Task<ActionResult> Register([FromBody] RegistrationFormDto requestBody)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var usernameIsTaken = await _userService.IsUsernameTaken(requestBody.Username);
        if (usernameIsTaken) ModelState.AddModelError("username",
            $"Имя пользователя {requestBody.Username} уже занято.");

        var emailIsTaken = await _userService.IsEmailTaken(requestBody.Email);
        if (emailIsTaken) ModelState.AddModelError("email",
            "Данная электронная почта уже занята.");

        if (!ModelState.IsValid) return Conflict(ModelState);

        var user = new User
        {
            Username = requestBody.Username,
            Email = requestBody.Email,
            FullName = requestBody.FullName,
            JobTitle = requestBody.JobTitle,
        };
        await user.SetPassword(requestBody.Password);
        await _userService.CreateUserAsync(user);

        var token = JwtTokenManager.IssueToken(user);
        
        return Ok(token);
    }
}