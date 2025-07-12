using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class GameReadyModel
{
    public event Action<string> UpdateReadyMessage = null;
    
    private readonly string GAMEREADY = "준비버튼을 눌러주세요.";
    private readonly string GAMEREADYCOMPLETE = "다른 플레이어를 기다리는 중이에요!";

    public void GameReadyStateChange(bool isReady)
    {
        if(isReady)
            UpdateReadyMessage?.Invoke(GAMEREADYCOMPLETE);
        else
            UpdateReadyMessage?.Invoke(GAMEREADY);
        
        SendReadyState(isReady);
    }

    private async void SendReadyState(bool isReady)
    {
        string readyValue = isReady ? "true" : "false";
        string lobbyId = PlayerSession.Instance.CurrentLobby.Id;
        string playerId = PlayerSession.Instance.PlayerId;

        await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, new UpdatePlayerOptions
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {
                    "ready",
                    new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member,
                        value: readyValue)
                }
            }
        });
    }
}