namespace CrazyBombChat.Models;

public class ChatMessage
{
    public string type { get; set; } = "";
    public string lobbyId { get; set; } = "";
    public string user { get; set; } = "";
    public string content { get; set; } = "";
}