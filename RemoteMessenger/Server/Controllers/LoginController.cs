using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RemoteMessenger.Server.Models;
using RemoteMessenger.Server.Util;
using RemoteMessenger.Shared;

namespace RemoteMessenger.Server.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{

    private readonly ILogger<LoginController> _logger;
    private readonly MessengerContext _context;
    private readonly IConfiguration _configuration;

    public LoginController(MessengerContext context, ILogger<LoginController> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost(Name = "Login")]
    public async Task<ActionResult<string>> Login(LoginUserDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == request.Username);
        if (user is null) return BadRequest($"User {request.Username} was not found");
        if (!await VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            return BadRequest("Password is wrong");
        _logger.LogInformation($"{user.Username} with id {user.Id} have logged in.");
        return Ok(CreateToken(user));
    }

    private async Task<bool> VerifyPasswordHash(string password, byte[] passHash, byte[] passSalt)
    {
        using var hmac = new HMACSHA512(passSalt);
        var passStream = new MemoryStream(Encoding.UTF8.GetBytes(password));
        var computeHash = await hmac.ComputeHashAsync(passStream);
        return computeHash.SequenceEqual(passHash);
    }

    private async Task<string> CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
        };
        var key = JwtTokenManager.Key;
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            issuer: $"{Request.Scheme}://{Request.Host}",
            claims: claims,
            expires: DateTime.Now.AddDays(14),
            signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}