using Microsoft.AspNetCore.Mvc;
using RemoteServer.Models.Shared;
using RemoteServer.Util;
using RemoteServer.Repositories;
using RemoteServer.Services;

namespace RemoteServer.Controllers.Authentication;

[ApiController]
[Route("/auth/register")]
public class RegisterController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly JwtTokenManager _jwtTokenManager;

    public RegisterController(UserRepository userRepository, JwtTokenManager jwtTokenManager)
    {
        _userRepository = userRepository;
        _jwtTokenManager = jwtTokenManager;
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
        
        var usernameIsTaken = await _userRepository.IsUsernameTaken(requestBody.Username);
        if (usernameIsTaken) ModelState.AddModelError("username",
            $"Имя пользователя {requestBody.Username} уже занято.");

        if (!ModelState.IsValid) return Conflict(ModelState);

        var user = new User
        {
            Username = requestBody.Username,
            FullName = requestBody.FullName,
            JobTitle = requestBody.JobTitle,
        };
        await user.SetPassword(requestBody.Password);
        await _userRepository.CreateUserAsync(user);

        var token = _jwtTokenManager.IssueToken(user);
        
        return Ok(token);
    }
}