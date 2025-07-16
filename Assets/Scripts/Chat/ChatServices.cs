using System;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

public class ChatServices : MonoBehaviour
{
    private WebSocket websocket;

    private string serverUrl = "ws://127.0.0.1:5267/ws";
    private string lobbyId;
    private string username;

    public event Action<string, string> OnChatReceived;

    public void InitChatService(string _lobbyId, string _username)
    {
        lobbyId = _lobbyId;
        username = _username;

        ConnectToServer();
    }
    
    private async Task ConnectToServer()
    {
        websocket = new WebSocket(serverUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("✅ Connected to server");

            var joinMsg = new ChatMessage
            {
                type = "join",
                lobbyId = lobbyId,
                user = username,
                content = ""
            };
            SendMessage(joinMsg);
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError($"❌ WebSocket error: {e}");
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("🔌 Connection closed");
        };

        try
        {
            Debug.Log($"📡 Connecting to: {serverUrl}");
            await websocket.Connect();
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Connect() Exception: {ex.Message}");
        }
    }

    private void Update()
    {
        websocket?.DispatchMessageQueue();
    }

    public async void SendChat(string content)
    {
        if (websocket == null || websocket.State != WebSocketState.Open)
            return;

        var msg = new ChatMessage
        {
            type = "chat",
            lobbyId = lobbyId,
            user = username,
            content = content
        };

        SendMessage(msg);
    }

    private async void SendMessage(ChatMessage message)
    {
        string json = JsonUtility.ToJson(message);
        await websocket.SendText(json);
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
            await websocket.Close();
    }

    [Serializable]
    public class ChatMessage
    {
        public string type;
        public string lobbyId;
        public string user;
        public string content;
    }
}
