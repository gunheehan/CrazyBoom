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
    public event Action<bool> OnEnterdLobby;
    public event Action<List<Lobby>> OnLobbyListUpdated; // 로비 목록이 업데이트될 때 호출될 콜백
    public event Action<Lobby> joinLobbyEvent;
    public event Action leaveLobbyEvent;

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

    private Lobby currentLobby;
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
        Debug.Log("로그인 시도중");
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

        Debug.Log("로그인 완료: " + playerName);
        OnEnterdLobby?.Invoke(true);
        //StartCoroutine(UpdateLobbyListCoroutine());
        await RefreshLobbyList();
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

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            Debug.Log("로비 생성 성공: " + currentLobby.Id);

            // 하트비트(유지) 시작
            heartbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(currentLobby.Id));

            joinLobbyEvent?.Invoke(currentLobby);
            PlayerSession.Instance.Initialize(playerId, playerName, currentLobby);
            SceneManager.LoadScene("Game");
            //await RefreshLobbyList();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 생성 실패: " + e.Message);
        }
    }

    // 3️⃣ 로비 검색 후 참여
    public async Task<Lobby> JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
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

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
            Debug.Log("로비 참가 성공: " + currentLobby.Id);
            return currentLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 참가 실패: " + e.Message);
            return null;
        }
    }
    
    public async void LeaveLobby()
    {
        if(currentLobby == null) return;

        try
        {
            MigrateHost();
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
            currentLobby = null;

            leaveLobbyEvent?.Invoke();
            await RefreshLobbyList();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"{e}");
        }
    }

    // 4️⃣ 공개된 로비 목록 검색
    public async Task<List<Lobby>> QueryLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
            Debug.Log($"검색된 로비 수: {response.Results.Count}");

            return response.Results;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 검색 실패: " + e.Message);
            return new List<Lobby>();
        }
    }
    
    // 🔹 일정 시간마다 로비 목록을 갱신 (5초마다)
    private IEnumerator UpdateLobbyListCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            RefreshLobbyList();
        }
    }

    // 🔹 로비 목록 새로고침 함수
    public async Task RefreshLobbyList()
    {
        Debug.Log("로비 리스트 업데이트");
        List<Lobby> lobbies = await QueryLobbies();
        OnLobbyListUpdated?.Invoke(lobbies); // UI 업데이트를 위해 이벤트 호출
    }

    // 5️⃣ 로비 유지 (하트비트)
    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId)
    {
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSeconds(15);
        }
    }
    
    private bool IsLobbyhost()
    {
        return currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    
    private async void MigrateHost()
    {
        if(!IsLobbyhost() || currentLobby.Players.Count <= 1)  return;
        try
        {
            currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions{
                HostId = currentLobby.Players[1].Id
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"{e}");
        }
    }
}
