using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyUIEventBridge : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] uiComponents;

    private ILobbyUI[] lobbyUIList;
    private LocalLobby currentLobby;
    
    private void Awake()
    {
        lobbyUIList = uiComponents.OfType<ILobbyUI>().ToArray();
    }

    public void SetUIEvent(LocalLobby localLobby)
    {
        foreach (ILobbyUI lobbyUI in lobbyUIList)
        {
            lobbyUI.SubScrive(localLobby);
        }

        currentLobby = localLobby;
    }

    private void OnDestroy()
    {
        if (lobbyUIList == null)
            return;
        
        foreach (ILobbyUI lobbyUI in lobbyUIList)
        {
            lobbyUI.DisSubScrive(currentLobby);
        }

        lobbyUIList = null;
    }
}
