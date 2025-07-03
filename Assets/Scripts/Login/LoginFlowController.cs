using UnityEngine;

public class LoginFlowController : MonoBehaviour, ILoginListener
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject roomListPanel;

    public void OnLoginSuccess(string playerId)
    {
        loginPanel.SetActive(false);
        roomListPanel.SetActive(true);
        Debug.Log($"로그인 성공! PlayerId: {playerId}");
    }
}