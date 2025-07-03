using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private InputField playerName_inputfield;
    [SerializeField] private Button login_button;
    [SerializeField] private Text errorMessage_text;
    [SerializeField] private MonoBehaviour loginRequesterObject;

    private ILoginRequester loginRequester;

    private void Awake()
    {
        loginRequester = loginRequesterObject as ILoginRequester;

        if (loginRequester == null)
            Debug.LogError("ILoginRequester를 구현한 객체가 아닙니다.");
    }
    
    void Start()
    {
        login_button.onClick.AddListener(OnclickLogin);
        errorMessage_text.text = string.Empty;
    }

    private void OnclickLogin()
    {
        if (string.IsNullOrEmpty(playerName_inputfield.text))
        {
            errorMessage_text.text= "닉네임을 입력해주세요.";
            return;
        }

        errorMessage_text.text = string.Empty;

        InitializeUnityServices(playerName_inputfield.text);
    }

    private void OnConnectedServer(bool isEnter)
    {
        gameObject.SetActive(!isEnter);
    }
    
    // 1️⃣ Unity Gaming Services 초기화
    public async Task InitializeUnityServices(string playername)
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            string playerId = AuthenticationService.Instance.PlayerId;
            PlayerSession.Instance.Initialize(playerId, playername);
            loginRequester?.RequestLogin(playername);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
