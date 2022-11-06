using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using RemoteMessenger.Server.Models;
using RemoteMessenger.Shared;

namespace RemoteMessenger.Server.Controllers;

[ApiController]
[Route("api/register")]
public class RegisterController : ControllerBase
{
    private readonly MessengerContext _context;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(ILogger<RegisterController> logger, MessengerContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost(Name = "RegisterUser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> Register(RegisterUserDto request)
    {
        var isUsernameTaken = await _context.Users.AnyAsync(user => user.Username == request.Username);
        if (isUsernameTaken) return BadRequest($"Username {request.Username} is taken");
        
        var registerCode = await _context.RegisterCodes.FirstOrDefaultAsync(
            code => request.RegistrationCode == code.Code);
        if (registerCode is null) return BadRequest("Registration code doesn't exist");

        var hashSalt = await CreatePasswordHash(request.Password);
        var user = new User
        {
            Username = request.Username.ToLower(),
            FullName = request.FullName,
            JobTitle = request.JobTitle,
            Role = registerCode.Role,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            PasswordHash = hashSalt.Hash,
            PasswordSalt = hashSalt.Salt
        };
        
        _context.RegisterCodes.Remove(registerCode);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"{user.Username} was registered by code: {request.RegistrationCode}");
        return Ok("User was registered");
    }
    
    private static async Task<HashSalt> CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(password));
        return new HashSalt
        {
            Salt = hmac.Key,
            Hash = await hmac.ComputeHashAsync(stream)
        };
    }

    private struct HashSalt
    {
        public byte[] Hash { get; init; }
        public byte[] Salt { get; init; }
    }
}