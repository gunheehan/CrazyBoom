using System;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private Text lobbyname_txt;
    [SerializeField] private Text lobbyplayerinfo_txt;
    [SerializeField] private Button lobby_btn;

    private LobbyList lobbyList;
    private Lobby lobby;

    private void Start()
    {
        lobby_btn.onClick.AddListener(Join);
    }

    public void Initialise(LobbyList lobbylist, Lobby lobby)
    {
        Debug.LogError("이름 : " + lobby.Name);
        lobbyList = lobbylist;
        this.lobby = lobby;

        lobbyname_txt.text = this.lobby.Name;
        lobbyplayerinfo_txt.text = $"{this.lobby.Players.Count}/{lobby.MaxPlayers}";
        
        gameObject.SetActive(true);
    }

    private void Join()
    {
        lobbyList.JoinAsync(lobby);
    }
}
