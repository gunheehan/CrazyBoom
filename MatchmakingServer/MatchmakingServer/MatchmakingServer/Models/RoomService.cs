using MatchmakingServer.Models;

namespace MatchmakingServer.Services;

public class RoomService
{
    private readonly Dictionary<string, Room> _rooms = new();

    public Room CreateRoom(string name, int maxPlayers)
    {
        var room = new Room { Name = name, MaxPlayers = maxPlayers };
        _rooms[room.Id] = room;
        return room;
    }

    public Room? JoinRoom(string roomId, string playerId)
    {
        if (_rooms.TryGetValue(roomId, out var room))
        {
            if (room.Players.Count < room.MaxPlayers)
            {
                room.Players.Add(playerId);
                return room;
            }
        }
        return null;
    }

    public List<Room> GetRooms() => _rooms.Values.ToList();
}