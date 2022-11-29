using Microsoft.AspNetCore.Mvc;
using RemoteMessenger.Shared.Models;

namespace RemoteMessenger.Server.Controllers;

[Route("/messages/general")]
public class GeneralChatController : ControllerBase
{
    private MessengerContext _context;

    public GeneralChatController(MessengerContext context)
    {
        _context = context;
    }
    
    [HttpGet("{amount:int}")]
    public async Task<ActionResult< List<PublicMessage> >> GetLastMessages(int amount)
    {
        return Ok(await Task.Run(() => _context.PublicMessages.OrderBy(x => x.Id).Take(amount)));
    }
}