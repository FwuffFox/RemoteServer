using Microsoft.AspNetCore.Mvc;
using RemoteMessenger.Server.Services;
using RemoteMessenger.Server.Util;

namespace RemoteMessenger.Server.Controllers.Authentication;

[ApiController]
[Route("/auth/login")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;

    private readonly UserService _userService;

    public LoginController(ILogger<LoginController> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// Вход
    /// </summary>
    /// <param name="request">Данные для логина.</param>
    /// <response code="200"> Пользователь успешно вошёл. Возращает JWT
    /// токен для дальнейшей аутентификации</response>
    /// <response code="400"> Пользователь не смог войти. Возращаем ошибки. </response>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     POST /auth/login
    ///     {
    ///         "username": "@user",
    ///         "password": "password"
    ///     }
    /// </remarks>
    [HttpPost(Name = "Login")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest,
        Type = typeof(Dictionary<string, string[]>))]
    public async Task<IActionResult> Login([FromBody] LoginUserFormDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var user = await _userService.GetUserAsync(request.Username);
        if (user is null)
        {
            ModelState.AddModelError("username",
                $"Пользователя {request.Username} не существует.");
        }
        if (!ModelState.IsValid) return Unauthorized(ModelState);
        
        if (!await user!.IsPasswordValidAsync(request.Password))
            ModelState.AddModelError("password",
                $"Неправильный пароль.");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var token = JwtTokenManager.IssueToken(user);
        _logger.LogInformation($"{user.Username} with id {user.Id} have logged in.\n Token: {token}");
        return Ok(token);
    }
}