using UnityEngine;

public class PlayerListContoller : MonoBehaviour
{
    [SerializeField] private PlayerListUI ui;
    private PlayerListModel model = new PlayerListModel();
    

    private void OnEnable()
    {
        LobbyManager.Instance.joinLobbyEvent += model.OnUpdatePlayerList;
        model.UpdateListUI += ui.UpdateUI;
    }

    private void OnDisable()
    {
        LobbyManager.Instance.joinLobbyEvent -= model.OnUpdatePlayerList;
        model.UpdateListUI -= ui.UpdateUI;
    }
}
