using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RemoteMessenger.Server.Models;
using RemoteMessenger.Server.Util;
using RemoteMessenger.Shared;

namespace RemoteMessenger.Server.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    private readonly MessengerContext _context;

    private readonly ILogger<LoginController> _logger;

    public LoginController(MessengerContext context, ILogger<LoginController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost(Name = "Login")]
    public async Task<ActionResult<string>> Login(LoginUserDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == request.Username);
        if (user is null) return BadRequest($"User {request.Username} was not found");
        if (!await VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            return BadRequest("Password is wrong");
        
        var token = await JwtTokenManager.IssueToken(user);
        _logger.LogInformation($"{user.Username} with id {user.Id} have logged in.\n Token: {token}");
        return Ok(token);
    }

    private async Task<bool> VerifyPasswordHash(string password, byte[] passHash, byte[] passSalt)
    {
        using var hmac = new HMACSHA512(passSalt);
        var passStream = new MemoryStream(Encoding.UTF8.GetBytes(password));
        var computeHash = await hmac.ComputeHashAsync(passStream);
        return computeHash.SequenceEqual(passHash);
    }
}