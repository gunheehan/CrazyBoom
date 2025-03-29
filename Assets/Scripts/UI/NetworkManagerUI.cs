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
        start_btn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        
        startClient_btn.onClick.AddListener(StartClient);
        createlobby.onClick.AddListener(CreateLobby);
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
