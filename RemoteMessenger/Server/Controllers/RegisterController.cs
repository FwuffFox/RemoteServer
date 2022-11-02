using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteMessenger.Server.Models;

namespace RemoteMessenger.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class RegisterController : ControllerBase
{
    private ILogger<RegisterController> _logger;
    private MessengerContext _context;

    public RegisterController(ILogger<RegisterController> logger, MessengerContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost(Name = "RegisterUser")]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(RegisterForm form)
    {
        var registerKey = await _context.RegisterCodes.FirstOrDefaultAsync(r => r.Code == form.RegisterCode);
        if (registerKey is null) return BadRequest("RegisterKey is wrong or doesn't exist");
        _context.RegisterCodes.Remove(registerKey);
        if (!RSAEncryption.TryDecrypt_Bytes(form.RsaEncryptedPassword, out var decryptedBytes))
        {
            return BadRequest("Couldn't decrypt RSAEncryptedPassword");
        }
        using var sha256 = SHA256.Create();
        var hashedPasswordBytes = sha256.ComputeHash(decryptedBytes);
        var hashedPassword = Convert.ToBase64String(hashedPasswordBytes);
        var user = new User
        {
            PublicRsaKey = form.UserPublicRsaKey,
            Username = form.Name.ToLower(),
            HashedPassword = hashedPassword
        };
        _logger.LogInformation(string.Format("{0} with hashed password {1} was registered", form.Name, hashedPassword));
        _context.Users.Add(user);
        return Ok(user);
    }
}

public class RegisterForm
{
    public string Name { get; init; }
    public string UserPublicRsaKey { get; init; }
    public string RsaEncryptedPassword { get; init; }
    public string RegisterCode { get; init; }
}