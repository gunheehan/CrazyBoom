using System.Text.Json.Serialization;
using MatchmakingServer.Models; // Room 클래스 네임스페이스

[JsonSerializable(typeof(Room))]
[JsonSerializable(typeof(List<Room>))]
public partial class MyJsonContext : JsonSerializerContext
{
}