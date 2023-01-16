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
    /// Вход.
    /// </summary>
    /// <param name="request">Тело запроса.</param>
    /// <response code="200"> Пользователь успешно вошёл. Возращает JWT
    /// токен для дальнейшей аутентификации</response>
    /// <response code="401"> Пользователь не смог войти. Возращаем ошибки. </response>
    [HttpPost(Name = "Login")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized,
        Type = typeof(Dictionary<string, string[]>))]
    public async Task<IActionResult> Login(LoginUserFormDto request)
    {
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
        if (!ModelState.IsValid) return Conflict();
        
        var token = JwtTokenManager.IssueToken(user);
        _logger.LogInformation($"{user.Username} with id {user.Id} have logged in.\n Token: {token}");
        return Ok(token);
    }
}