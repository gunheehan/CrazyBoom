using UnityEngine;
using Unity.Netcode;

public class ServerBootstrap : MonoBehaviour
{
    void Start()
    {
        // 서버 모드로 실행될 때만 StartServer
        if (Application.isBatchMode)
        {
            Debug.Log("Starting Server");
            NetworkManager.Singleton.StartServer();
        }
        else
        {
            Debug.Log("Startring as Client or Host (Editor/Standalone)");
        }
    }
}