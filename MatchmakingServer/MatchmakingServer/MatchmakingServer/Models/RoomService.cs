using MatchmakingServer.Models;

namespace MatchmakingServer.Services;

public class RoomService
{
    private readonly Dictionary<string, Room> _rooms = new();
    private readonly object _lock = new();

    public Room CreateRoom(string name, int maxPlayers)
    {
        var roomId = Guid.NewGuid().ToString();
        var room = new Room
        {
            Id = roomId,
            Name = name,
            MaxPlayers = maxPlayers,
            Players = new List<string>()
        };

        lock (_lock)
        {
            _rooms.Add(roomId, room);
        }

        return room;
    }


    public Room? JoinRoom(string roomId, string playerId)
    {
        lock (_lock)
        {
            if (_rooms.TryGetValue(roomId, out var room))
            {
                if (room.Players.Contains(playerId))
                    return room; // 이미 참가 중이면 OK 반환

                if (room.Players.Count >= room.MaxPlayers)
                    return null; // 방이 가득 참

                room.Players.Add(playerId);
                return room;
            }

            return null; // 방 없음
        }
    }


    public List<Room> GetRooms() => _rooms.Values.ToList();
}