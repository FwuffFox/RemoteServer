using Microsoft.AspNetCore.Mvc;
using RemoteServer.Models.Shared;
using RemoteServer.Util;
using RemoteServer.Repositories;

namespace RemoteServer.Controllers;

[Route("/user")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserRepository _userRepository;
    
    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet(Name = "Get User")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status404NotFound,
        Type = typeof(string))]
    public async Task<IActionResult> GetUser([FromQuery] int? id = null, [FromQuery] string? username = null)
    {
        if (id is null && username is null)
        {
            username = await HttpContext.User.GetUniqueNameAsync();
        }
        
        var user = id is not null ? 
            await _userRepository.GetUserAsync(id.Value) : 
            await _userRepository.GetUserAsync(username!);
        
        if (user is null) return NotFound("No such user");
        return Ok(user);
    }
    
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("get_all_users")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _userRepository.GetAllUsersAsync());
    }
}