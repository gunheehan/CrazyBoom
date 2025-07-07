using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyConnectModel
{
    public async Task CreateLobby(string lobbyName, int maxPlayers)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player
                (
                    id: PlayerSession.Instance.PlayerId,
                    data: new Dictionary<string, PlayerDataObject>
                    {
                        { "nickname", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerSession.Instance.PlayerName) }
                    }
                ),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Normal") },
                }
            };

            var currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            
            PlayerSession.Instance.SetCurrentLobby(currentLobby);
            SceneManager.LoadScene("Game");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 생성 실패: " + e.Message);
        }
    }
    
    public async Task JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = new Player
                (
                    id: PlayerSession.Instance.PlayerId,
                    data: new Dictionary<string, PlayerDataObject>
                    {
                        { "nickname", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerSession.Instance.PlayerName) }
                    }
                )
            };

            var currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyCode, options);
            
            PlayerSession.Instance.SetCurrentLobby(currentLobby);
            SceneManager.LoadScene("Game");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 참가 실패: " + e.Message);
        }
    }
}
