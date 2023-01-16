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

    [HttpPost(Name = "RegisterUser")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status409Conflict,
        Type = typeof(Dictionary<string, string[]>))]
    public async Task<ActionResult> Register(RegistrationFormDto requestBody)
    {
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