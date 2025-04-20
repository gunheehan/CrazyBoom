using UnityEngine;
using Unity.Netcode;

public class ServerBootstrap : MonoBehaviour
{
    void Start()
    {
        // 서버 모드로 실행될 때만 StartServer
        if (Application.isBatchMode)
        {
            NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().SetConnectionData("0.0.0.0", 7777);
            NetworkManager.Singleton.StartServer();
        }
    }
}