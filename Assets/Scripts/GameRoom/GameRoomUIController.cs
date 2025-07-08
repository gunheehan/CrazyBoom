using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRoomUIController : MonoBehaviour
{
    [SerializeField] private PlayerListUI playerListUI;

    private List<IPresenter> presenters;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        presenters = new List<IPresenter>();

        var playerListModel = new PlayerListModel();
        PlayerListPresenter playerListPresenter = new PlayerListPresenter(playerListUI, playerListModel);
        presenters.Add(playerListPresenter);

        foreach (var presenter in presenters)
        {
            presenter.Subscribe();
        }
    }

    private void OnDestroy()
    {
        foreach (var presenter in presenters)
        {
            presenter.UnSubscribe();
        }
    }
}
