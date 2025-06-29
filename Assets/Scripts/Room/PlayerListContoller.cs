using UnityEngine;

public class PlayerListContoller : MonoBehaviour
{
    [SerializeField] private PlayerListUI ui;
    [SerializeField] private GameRoomManager roommanager;
    private PlayerListModel model = new PlayerListModel();

    private void Start()
    {
        model.OnUpdatePlayerList(PlayerSession.Instance.CurrentLobby);
        OnEnable();
    }
    
    private void OnEnable()
    {
        roommanager.OnEnteredPlayer += model.OnAddPlayer;
        model.UpdateListUI += ui.UpdateUI;
        model.AddListUI += ui.AddListUI;
    }

    private void OnDisable()
    {
        roommanager.OnEnteredPlayer -= model.OnAddPlayer;
        model.UpdateListUI -= ui.UpdateUI;
        model.AddListUI -= ui.AddListUI;
    }
}
