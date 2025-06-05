using System;
using UnityEngine;

public class LobbyListPresenter : MonoBehaviour
{
    [SerializeField] private LobbyListUI ui;

    private LobbyListModel model;
    
    void Start()
    {
        model = new LobbyListModel();
    }

    private void OnEnable()
    {
        LobbyManager.Instance.OnEnterdLobby += OnEnteredLobby;
    }

    private void OnDisable()
    {
        LobbyManager.Instance.OnEnterdLobby -= OnEnteredLobby;
    }

    private void OnEnteredLobby(bool isEnter)
    {
        ui.gameObject.SetActive(isEnter);
    }
}
