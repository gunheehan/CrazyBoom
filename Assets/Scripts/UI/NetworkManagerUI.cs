using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button start_btn;
    [SerializeField] private Button startClient_btn;
    [SerializeField] private Button createlobby;
    private string lobbyID;

    private void Start()
    {
        // start_btn.onClick.AddListener(() =>
        // {
        //     NetworkManager.Singleton.StartServer();
        // });
        
        //startClient_btn.onClick.AddListener(ConnectToServer);
        createlobby.onClick.AddListener(CreateLobby);
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            Debug.Log($"서버에서 클라이언트 {clientId} 접속 확인");
        };
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log("클라이언트: 서버 연결 성공!");
            }
        };
    }
    
    public string serverIP = "175.116.241.172";  // 서버 IP 기본값
    public ushort serverPort = 7777;       // 서버 포트 기본값

    // UI 버튼 등에서 호출할 수 있도록 public 함수
    public void ConnectToServer()
    {
        Debug.Log(NetworkManager.Singleton == null ? "NetworkManager.Singleton is NULL" : "NetworkManager.Singleton OK");

        // NetworkManager의 Transport 설정을 서버 IP/포트로 지정
        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        transport.ConnectionData.Address = "175.116.241.172";
        transport.ConnectionData.Port = serverPort;

        // 클라이언트 시작
        NetworkManager.Singleton.StartClient();

        Debug.Log($"서버에 접속 시도: {serverIP}:{serverPort}");
    }

    private async void StartClient()
    {
        List<Unity.Services.Lobbies.Models.Lobby> lobbies = await LobbyManager.Instance.QueryLobbies();

        string lobbyListText = string.Empty;
        foreach (var lobby in lobbies)
        {
            lobbyListText += $"{lobby.Name} - {lobby.MaxPlayers}명 (현재 {lobby.Players.Count}명)\n";
        }
        
        Debug.Log(lobbyListText);
        //await ClientManager.instance.StartClient("테스트");
    }

    private async void CreateLobby()
    {
        await LobbyManager.Instance.CreateLobby("test", 4);
    }
    
    private async void EnterLobby()
    {
        try
        {
            string lobbyName = "new lobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "Joincode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: Joincode
                    )
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            lobbyID = lobby.Id;

            StartCoroutine(HeartBeatLobbyCoroutine(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    IEnumerator HeartBeatLobbyCoroutine(float waitTime)
    {
        var delay = new WaitForSeconds(waitTime);
        while (true)
        {
            var task = LobbyService.Instance.SendHeartbeatPingAsync(lobbyID);
            yield return delay;
        }
    }

    private const string Joincode = "JoinCode";
}
