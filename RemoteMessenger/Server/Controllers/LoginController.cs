using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteMessenger.Server.Models;

namespace RemoteMessenger.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{

    private ILogger<LoginController> _logger;
    private MessengerContext _context;

    public LoginController(MessengerContext context, ILogger<LoginController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public struct LoginForm
    {
        public string Username { get; set; }
        public string RsaEncryptedPassword { get; set; }
    }
    
    [HttpPost(Name = "Login")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Post(LoginForm form)
    {
        var username = form.Username.ToLower();
        if (!RSAEncryption.TryDecrypt_Bytes(form.RsaEncryptedPassword, out var decryptedBytes))
        {
            return BadRequest("Couldn't decrypt password");
        }
        using var sha = SHA256.Create();
        var hashedPasswordBytes = sha.ComputeHash(decryptedBytes);
        var hashedPassword = Convert.ToBase64String(hashedPasswordBytes);

        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username 
                                                                    && user.HashedPassword == hashedPassword);
        
        if (user is null)
        {
            return Forbid();
        }

        var guid = new Guid();
        Response.Cookies.Append("session-id", guid.ToString());
        return Ok();
    }
}