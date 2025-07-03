using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class UnityLoginService : MonoBehaviour, ILoginRequester
{
    [SerializeField] private MonoBehaviour loginListenerObject;

    private ILoginListener loginListener;

    private void Awake()
    {
        loginListener = loginListenerObject as ILoginListener;

        if (loginListener == null)
            Debug.LogError("ILoginListener를 구현한 객체가 아닙니다.");
    }

    public async void RequestLogin(string playerName)
    {
        try
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            string playerId = AuthenticationService.Instance.PlayerId;
            PlayerSession.Instance.Initialize(playerId, playerName);

            loginListener?.OnLoginSuccess(playerId);
        }
        catch (AuthenticationException ae)
        {
            Debug.LogError($"Authentication error: {ae}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Login failed: {e}");
        }
    }
}