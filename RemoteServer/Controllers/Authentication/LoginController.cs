using Microsoft.AspNetCore.Mvc;
using RemoteServer.Models.Shared;
using RemoteServer.Services;

namespace RemoteServer.Controllers.Authentication;

[ApiController]
[Route("/auth/login")]
public class LoginController : ControllerBase
{
    private readonly JwtTokenManager _jwtTokenManager;
    private readonly ILogger<LoginController> _logger;
    private readonly UserRepository _userRepository;

    public LoginController(ILogger<LoginController> logger, UserRepository userRepository,
        JwtTokenManager jwtTokenManager)
    {
        _logger = logger;
        _userRepository = userRepository;
        _jwtTokenManager = jwtTokenManager;
    }

    /// <summary>
    ///     Вход
    /// </summary>
    /// <param name="request">Данные для логина.</param>
    /// <response code="200">
    ///     Пользователь успешно вошёл. Возращает JWT
    ///     токен для дальнейшей аутентификации
    /// </response>
    /// <response code="400"> Пользователь не смог войти. Возращаем ошибки. </response>
    /// <remarks>
    ///     Пример запроса:
    ///     POST /auth/login
    ///     {
    ///     "username": "@user",
    ///     "password": "password"
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

        var user = await _userRepository.GetUserAsync(request.Username);
        if (user is null)
            ModelState.AddModelError("username",
                $"Пользователя {request.Username} не существует.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (!await user!.IsPasswordValidAsync(request.Password))
            ModelState.AddModelError("password",
                "Неправильный пароль.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var token = _jwtTokenManager.IssueToken(user);
        _logger.LogInformation($"{user.Username} with id {user.UserId} have logged in.\n Token: {token}");
        return Ok(token);
    }
}