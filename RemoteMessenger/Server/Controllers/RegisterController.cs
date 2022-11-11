using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using RemoteMessenger.Server.Models;
using RemoteMessenger.Server.Services;

namespace RemoteMessenger.Server.Controllers;

[ApiController]
[Route("api/register")]
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
    public async Task<ActionResult<string>> Register(RegisterUserDto request)
    {
        var usernameIsTaken = await _userService.IsUsernameTaken(request.Username);
        if (usernameIsTaken) return BadRequest($"Username {request.Username} is taken");
        
        var registerCode = await _userService.GetRegistrationCodeAsync(request.RegistrationCode);
        if (registerCode is null) return BadRequest("Registration code doesn't exist");
        
        var user = new User
        {
            Username = request.Username.ToLower(),
            FullName = request.FullName,
            JobTitle = request.JobTitle,
            Role = registerCode.Role,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
        };
        await user.SetPassword(request.Password);
        await _userService.CreateUser(user, registerCode);
        
        return Ok("User was registered");
    }
    

    private struct HashSalt
    {
        public byte[] Hash { get; init; }
        public byte[] Salt { get; init; }
    }
}