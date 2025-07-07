using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
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
            await localLobby.RefreshLobbyList();
            
            PlayerSession.Instance.SetCurrentLobby(currentLobby);
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
            
            PlayerSession.Instance.SetCurrentLobby(currentLobby);
            SceneManager.LoadScene("Game");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 참가 실패: " + e.Message);
        }
    }
}
