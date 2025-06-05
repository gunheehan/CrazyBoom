using UnityEngine;

public class PlayerListContoller : MonoBehaviour
{
    [SerializeField] private PlayerListUI ui;
    private PlayerListModel model = new PlayerListModel();

    private void Start()
    {
        model.OnUpdatePlayerList(PlayerSession.Instance.CurrentLobby);
    }
    
    private void OnEnable()
    {
        model.UpdateListUI += ui.UpdateUI;
    }

    private void OnDisable()
    {
        model.UpdateListUI -= ui.UpdateUI;
    }
}
