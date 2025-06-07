using System;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private InputField playerName_inputfield;

    [SerializeField] private Button login_button;

    [SerializeField] private Text errorMessage_text;
    
    void Start()
    {
        login_button.onClick.AddListener(OnclickLogin);
        errorMessage_text.text = string.Empty;
    }

    private void OnEnable()
    {
        LobbyManager.Instance.OnEnterdLobby += OnEnterdLobby;
    }

    private void OnDisable()
    {
        LobbyManager.Instance.OnEnterdLobby -= OnEnterdLobby;
    }

    private void OnclickLogin()
    {
        if (string.IsNullOrEmpty(playerName_inputfield.text))
        {
            errorMessage_text.text= "닉네임을 입력해주세요.";
            return;
        }

        errorMessage_text.text = string.Empty;

        LobbyManager.Instance.InitializeUnityServices(playerName_inputfield.text);
    }

    private void OnEnterdLobby(bool isEnter)
    {
        gameObject.SetActive(!isEnter);
    }
}
