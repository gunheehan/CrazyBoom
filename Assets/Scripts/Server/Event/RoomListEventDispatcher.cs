using System;
using System.Collections.Generic;

public static class RoomListEventDispatcher
{
    public static event Action<List<string>> OnRoomListUpdated;

    public static void InvokeRoomListUpdated(List<string> roomList)
    {
        OnRoomListUpdated?.Invoke(roomList);
    }
}