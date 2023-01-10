using Microsoft.AspNetCore.Mvc;
using RemoteMessenger.Server.Util;
using RemoteMessenger.Shared.Models;

namespace RemoteMessenger.Server.Controllers;

[Route("/private_chats")]
public class PrivateChatsController : ControllerBase
{
    private readonly MessengerContext _context;

    public PrivateChatsController(MessengerContext context)
    {
        _context = context;
    }
    
    [HttpGet("getMyChats")]
    public async Task<ActionResult<List<PrivateChat>>> GetMyChats()
    {
        var username = await HttpContext.User.GetUniqueNameAsync();
        var result =
            await Task.Run(() => _context.PrivateChats
                .Where(chat => chat.Sender.Username == username || chat.Receiver.Username == username)
                .OrderBy(chat => chat.Id)
                .ToList());
        return Ok(result);
    }

    [HttpGet("{chatId:int}")]
    public async Task<ActionResult<List<PrivateMessage>>> GetPrivateMessages(int chatId)
    {
        var username = await HttpContext.User.GetUniqueNameAsync();
        var chat = await _context.PrivateChats.FirstOrDefaultAsync(chat => chat.Id == chatId);
        if (chat is null) return NotFound("No such chat exists");
        if (!await chat.IsUserInChat(username)) return Unauthorized("User is not in chat");
        return Ok(chat.Messages);
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<List<PrivateMessage>>> GetPrivateMessages(string username)
    {
        var myUsername = await HttpContext.User.GetUniqueNameAsync();
        var chat = await _context.PrivateChats.FirstOrDefaultAsync(
            chat => chat.Sender.Username == username ||  chat.Receiver.Username == username
            && chat.Sender.Username == myUsername ||  chat.Receiver.Username == myUsername);
        if (chat is null) return NotFound("No such chat exists");
        if (!await chat.IsUserInChat(myUsername)) return Forbid();
        return Ok(chat.Messages);
    }
    
    // TODO: New DMS Controller
    [HttpGet("create")]
    public async Task<ActionResult> CreateChat(string username)
    {
        var myUsername = await HttpContext.User.GetUniqueNameAsync();
        var chat = await _context.PrivateChats.FirstOrDefaultAsync(
            chat => chat.Sender.Username == username ||  chat.Receiver.Username == username
                && chat.Sender.Username == myUsername ||  chat.Receiver.Username == myUsername);
        if (chat is not null) return Forbid();
        var firstUser = await _context.Users.FirstOrDefaultAsync(c => c.Username == myUsername);
        var secondUser = await _context.Users.FirstOrDefaultAsync(c => c.Username == username);
        if (secondUser is null) return NotFound();
        var newChat = new PrivateChat
        {
            Sender = firstUser,
            Receiver = secondUser,
            Messages = new List<PrivateMessage>(100)
        };
        await _context.PrivateChats.AddAsync(newChat);
        await _context.SaveChangesAsync();
        return Ok();
    }
}