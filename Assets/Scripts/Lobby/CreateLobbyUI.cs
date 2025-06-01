using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private InputField lobbyname_inputfield;

    [SerializeField] private InputField lobbymenber_inputfield;

    [SerializeField] private Button create_button;

    void Start()
    {
        create_button.onClick.AddListener(OnClickCreateLobby);
    }

    private void OnClickCreateLobby()
    {
        if (lobbyname_inputfield.text == null || lobbymenber_inputfield.text == null)
        {
            Debug.Log("생성 항목을 모두 채워주시오.");
        }

        LobbyManager.Instance.CreateLobby(lobbyname_inputfield.text, int.Parse(lobbymenber_inputfield.text));
    }
}
