using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RemoteMessenger.Server.Models;
using RemoteMessenger.Server.Services;
using RemoteMessenger.Server.Util;
using RemoteMessenger.Shared;

namespace RemoteMessenger.Server.Controllers;

[ApiController]
[Route("api/auth/login")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;

    private readonly UserService _userService;

    public LoginController(ILogger<LoginController> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost(Name = "Login")]
    public async Task<ActionResult<string>> Login(LoginUserFormDto request)
    {
        var user = await _userService.GetUserAsync(request.Username);
        if (user is null) 
            return BadRequest($"User {request.Username} was not found");
        
        if (!await user.IsPasswordValidAsync(request.Password))
            return BadRequest("Password is wrong");
        
        var token = JwtTokenManager.IssueToken(user);
        _logger.LogInformation($"{user.Username} with id {user.Id} have logged in.\n Token: {token}");
        return Ok(token);
    }
}