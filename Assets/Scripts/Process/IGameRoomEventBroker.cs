using System;
using Unity.Services.Lobbies.Models;

public interface IGameRoomEventBroker
{
    event Action<Player> PlayerJoined;
    event Action<string> PlayerLeft;
    event Action<Player, string, string> PlayerStateChanged;
    event Action<string> HostChanged;
}