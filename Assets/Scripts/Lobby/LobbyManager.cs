using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private LobbyUIEventBridge lobbyUIEventBridge;
    public event Action<bool> OnConnectedServer = null;
    private static LobbyManager instance;
    public static LobbyManager Instance
    {
        get
        {
            if (instance == null)
                instance = new LobbyManager();
            return instance;
        }
    }

    //private Lobby currentLobby;
    private LocalLobby localLobby;
    private Coroutine heartbeatCoroutine;
    private string playerId;
    private string playerName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 1️⃣ Unity Gaming Services 초기화
    public async Task InitializeUnityServices(string playername)
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            playerId = AuthenticationService.Instance.PlayerId;
            playerName = playername;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        OnConnectedServer?.Invoke(true);
    }

    // 2️⃣ 로비 생성
    public async Task CreateLobby(string lobbyName, int maxPlayers)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player
                (
                    id: playerId,
                    data: new Dictionary<string, PlayerDataObject>
                    {
                        { "nickname", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
                ),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Normal") },
                }
            };

            var currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            localLobby = new LocalLobby(currentLobby);
            lobbyUIEventBridge.SetUIEvent(localLobby);
            await localLobby.RefreshLobbyList();
            
            PlayerSession.Instance.Initialize(playerId, playerName, currentLobby);
            SceneManager.LoadScene("Game");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 생성 실패: " + e.Message);
        }
    }

    // 3️⃣ 로비 검색 후 참여
    public async Task JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = new Player
                (
                    id: playerId,
                    data: new Dictionary<string, PlayerDataObject>
                    {
                        { "nickname", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
                )
            };

            var currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyCode, options);
            localLobby = new LocalLobby(currentLobby);
            lobbyUIEventBridge.SetUIEvent(localLobby);
            
            PlayerSession.Instance.Initialize(playerId, playerName, currentLobby);
            SceneManager.LoadScene("Game");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 참가 실패: " + e.Message);
        }
    }
}
