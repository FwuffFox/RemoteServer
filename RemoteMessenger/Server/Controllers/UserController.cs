using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemoteMessenger.Server.Models;
using RemoteMessenger.Server.Services;

namespace RemoteMessenger.Server.Controllers;

[Route("user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUser([FromQuery] int? id = null, [FromQuery] string? username = null)
    {
        if (id is null && username is null) return BadRequest("No query passed");
        
        var user = id is not null ? 
            await _userService.GetUserAsync(id.Value) : 
            await _userService.GetUserAsync(username!);
        
        if (user is null) return BadRequest("No such user");
        return Ok(user);
    }
    
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("get_all_users")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _userService.GetAllUsersAsync());
    }
}