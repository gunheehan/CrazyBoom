using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRoomUIController : MonoBehaviour
{
    [SerializeField] private PlayerListUI playerListUI;
    [SerializeField] private GameReadyUI gameReadyUI;

    private List<IPresenter> presenters;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        presenters = new List<IPresenter>();

        PlayerListModel playerListModel = new PlayerListModel();
        PlayerListPresenter playerListPresenter = new PlayerListPresenter(playerListUI, playerListModel);
        presenters.Add(playerListPresenter);

        GameReadyModel gameReadyModel = new GameReadyModel();
        GameReadyPresenter gameReadyPresenter = new GameReadyPresenter(gameReadyUI, gameReadyModel);
        presenters.Add(gameReadyPresenter);

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
