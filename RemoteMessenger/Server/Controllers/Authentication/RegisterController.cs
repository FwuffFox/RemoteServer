using Microsoft.AspNetCore.Mvc;
using RemoteMessenger.Server.Services;
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register(RegistrationFormDto request)
    {
        var usernameIsTaken = await _userService.IsUsernameTaken(request.Username);
        if (usernameIsTaken) return BadRequest($"Username {request.Username} is taken");

        var user = new User
        {
            Username = request.Username.ToLower(),
            FullName = request.FullName,
            JobTitle = request.JobTitle,
        };
        await user.SetPassword(request.Password);
        await _userService.CreateUserAsync(user);
        
        return Ok("User was registered");
    }
}