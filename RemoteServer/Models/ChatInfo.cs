namespace RemoteServer.Models;

public class ChatInfo
{
    public required User OtherUser { get; set; }
    
    public required List<ChatMessage> MessagesFromMe { get; set; }
    
    public required List<ChatMessage> MessagesToMe { get; set; }
}