using Unity.Netcode;

public class ClientToServerRoomProxy
{
    public void RequestRoomList()
    {
        // 서버에 룸 목록을 요청
        if (NetworkManager.Singleton.IsClient)
        {
            var serverRoomManager = NetworkManager.Singleton.GetComponent<ServerRoomManager>();
            serverRoomManager?.RequestRoomListServerRpc(); // 서버에 룸 목록 요청
        }
    }

    public void RequestCreateRoom(string roomId)
    {
        // 서버에 룸 생성 요청
        if (NetworkManager.Singleton.IsClient)
        {
            var serverRoomManager = NetworkManager.Singleton.GetComponent<ServerRoomManager>();
            serverRoomManager?.RequestCreateRoomServerRpc(roomId); // 서버에 룸 생성 요청
        }
    }

    public void RequestJoinRoom(string roomId)
    {
        // 서버에 방 참가 요청
        if (NetworkManager.Singleton.IsClient)
        {
            var serverRoomManager = NetworkManager.Singleton.GetComponent<ServerRoomManager>();
            serverRoomManager?.RequestJoinRoomServerRpc(roomId); // 서버에 참가 요청
        }
    }
}