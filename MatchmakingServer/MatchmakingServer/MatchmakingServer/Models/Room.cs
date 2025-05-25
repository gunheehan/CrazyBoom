namespace MatchmakingServer.Models
{
    public class Room
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public int MaxPlayers { get; set; }
        public List<string> Players { get; set; } = new List<string>();  // 여기 Players로 맞춰줘야 함
    }

}