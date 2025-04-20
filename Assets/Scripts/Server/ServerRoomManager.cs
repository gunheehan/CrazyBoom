using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ServerRoomManager : NetworkBehaviour
{
    public static ServerRoomManager Instance;

    private Dictionary<string, Room> rooms = new();
    private Dictionary<ulong, string> clientRoomMap = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"🟢 클라이언트 {clientId} 접속");

        List<string> roomList = new(rooms.Keys);
        SendRoomListToClient(clientId, roomList);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"🔴 클라이언트 {clientId} 연결 종료");
        LeaveRoom(clientId);
    }

    private void SendRoomListToClient(ulong clientId, List<string> roomList)
    {
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { clientId }
            }
        };
        
        FixedString32Bytes[] roomArray = roomList.Select(name => (FixedString32Bytes)name).ToArray();

        RoomListUpdateClientRpc(roomArray, clientRpcParams);
    }

    private void SendRoomListToClients(List<string> roomList, List<ulong> clients)
    {
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds)
            }
        };
        
        FixedString32Bytes[] roomArray = roomList.Select(name => (FixedString32Bytes)name).ToArray();
        
        RoomListUpdateClientRpc(roomArray, clientRpcParams);
    }

    [ClientRpc]
    private void RoomListUpdateClientRpc(FixedString32Bytes[] roomArray, ClientRpcParams clientRpcParams = default)
    {
        List<string> roomList = roomArray.Select(name => name.ToString()).ToList();
        RoomListEventDispatcher.InvokeRoomListUpdated(roomList);
    }

    // -----------------------------
    // Room 구조체
    // -----------------------------
    public class Room
    {
        public string RoomId;
        public List<ulong> Clients = new();
        public event Action<List<string>, List<ulong>> OnRoomListChanged;

        public Room(string id)
        {
            RoomId = id;
        }

        public void AddClient(ulong clientId)
        {
            if (!Clients.Contains(clientId))
            {
                Clients.Add(clientId);
                NotifyChange();
            }
        }

        public void RemoveClient(ulong clientId)
        {
            if (Clients.Remove(clientId))
            {
                NotifyChange();
            }
        }

        public void NotifyChange()
        {
            OnRoomListChanged?.Invoke(GetRoomList(), Clients);
        }

        public List<string> GetRoomList()
        {
            return new List<string> { RoomId };
        }
    }

    // -----------------------------
    // Room 관리
    // -----------------------------
    public void CreateRoom(string roomId, ulong clientId)
    {
        if (!rooms.ContainsKey(roomId))
        {
            var newRoom = new Room(roomId);
            newRoom.OnRoomListChanged += SendRoomListToClients;
            newRoom.AddClient(clientId);

            rooms[roomId] = newRoom;
            clientRoomMap[clientId] = roomId;

            Debug.Log($"[ServerRoomManager] 룸 생성: {roomId}, 생성자: {clientId}");

            // 룸 생성 후, 룸 목록을 모든 클라이언트에게 업데이트
            SendRoomListToClients(new List<string>(rooms.Keys), new List<ulong>(clientRoomMap.Keys));
        }
    }

    public void JoinRoom(string roomId, ulong clientId)
    {
        if (rooms.TryGetValue(roomId, out var room))
        {
            room.AddClient(clientId);
            clientRoomMap[clientId] = roomId;

            Debug.Log($"[ServerRoomManager] 클라이언트 {clientId}가 룸 {roomId}에 참가함.");

            // 룸에 참여 후, 룸 목록을 모든 클라이언트에게 업데이트
            SendRoomListToClients(new List<string>(rooms.Keys), new List<ulong>(clientRoomMap.Keys));
        }
    }

    public void LeaveRoom(ulong clientId)
    {
        if (clientRoomMap.TryGetValue(clientId, out var roomId))
        {
            if (rooms.TryGetValue(roomId, out var room))
            {
                room.RemoveClient(clientId);
                clientRoomMap.Remove(clientId);

                Debug.Log($"[ServerRoomManager] 클라이언트 {clientId}가 룸 {roomId}에서 나감.");

                // 룸을 떠난 후, 룸 목록을 모든 클라이언트에게 업데이트
                SendRoomListToClients(new List<string>(rooms.Keys), new List<ulong>(clientRoomMap.Keys));

                if (room.Clients.Count == 0)
                {
                    room.OnRoomListChanged -= SendRoomListToClients;
                    rooms.Remove(roomId);
                    Debug.Log($"[ServerRoomManager] 룸 {roomId} 제거됨 (비었음)");
                }
            }
        }
    }

    // -----------------------------
    // 클라이언트 요청 처리
    // -----------------------------

    [ServerRpc(RequireOwnership = false)]
    public void RequestCreateRoomServerRpc(string roomId, ServerRpcParams rpcParams = default)
    {
        CreateRoom(roomId, rpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestJoinRoomServerRpc(string roomId, ServerRpcParams rpcParams = default)
    {
        JoinRoom(roomId, rpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestRoomListServerRpc(ServerRpcParams rpcParams = default)
    {
        var roomList = new List<string>(rooms.Keys);
        SendRoomListToClient(rpcParams.Receive.SenderClientId, roomList);
    }
}