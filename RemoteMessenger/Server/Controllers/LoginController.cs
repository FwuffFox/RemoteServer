using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteMessenger.Server.Models;
using RemoteMessenger.Shared;

namespace RemoteMessenger.Server.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{

    private readonly ILogger<LoginController> _logger;
    private readonly MessengerContext _context;

    public LoginController(MessengerContext context, ILogger<LoginController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost(Name = "Login")]
    public async Task<ActionResult<string>> Login(LoginUserDto request)
    {
        var userByUsername = await _context.Users.FirstOrDefaultAsync(user => user.Username == request.Username);
        if (userByUsername is null) return BadRequest($"User {request.Username} was not found");
        if (!await VerifyPasswordHash(request.Password, userByUsername.PasswordHash, userByUsername.PasswordSalt))
            return BadRequest("Password is wrong");
        _logger.LogInformation($"{userByUsername.Username} with id {userByUsername.Id} have logged in.");
        return Ok("Token there");
    }

    private async Task<bool> VerifyPasswordHash(string password, byte[] passHash, byte[] passSalt)
    {
        using var hmac = new HMACSHA512(passSalt);
        var passStream = new MemoryStream(Encoding.UTF8.GetBytes(password));
        var computeHash = await hmac.ComputeHashAsync(passStream);
        return computeHash.SequenceEqual(passHash);
    }
    
    
}