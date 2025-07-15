namespace CrazyBombChat.Models;

public class ChatMessage
{
    public string Type { get; set; } = "";
    public string LobbyId { get; set; } = "";
    public string User { get; set; } = "";
    public string Content { get; set; } = "";
}