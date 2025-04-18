using UnityEngine;
using Unity.Netcode;

public class ServerBootstrap : MonoBehaviour
{
    void Start()
    {
        // 서버 모드로 실행될 때만 StartServer
        if (Application.isBatchMode)
        {
            Debug.Log("✅ 서버 모드 감지됨 - StartServer 실행");
            NetworkManager.Singleton.StartServer();
        }
        else
        {
            Debug.Log("⚠️ 클라이언트 모드이므로 서버 실행 안 함");
        }
    }
}