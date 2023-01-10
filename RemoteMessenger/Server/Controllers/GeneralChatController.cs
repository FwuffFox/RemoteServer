using Microsoft.AspNetCore.Mvc;
using RemoteMessenger.Shared.Models;

namespace RemoteMessenger.Server.Controllers;

[Route("messages/general")]
public class GeneralChatController : ControllerBase
{
    private readonly MessengerContext _context;

    public GeneralChatController(MessengerContext context)
    {
        _context = context;
    }
    
    [HttpGet("{amount:int}")]
    public async Task<ActionResult< List<PublicMessage> >> GetLastMessages(int amount)
    {
        var result = await Task.Run(() =>
            _context.PublicMessages
            .Include(m => m.Sender)
            .OrderByDescending(x => x.Id)
            .Take(amount));
        return Ok(result.Reverse());
    }
}