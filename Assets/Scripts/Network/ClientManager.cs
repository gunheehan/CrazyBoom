using Unity.Netcode;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public string serverIp = "175.116.241.172"; // 로비에서 받은 서버 IP
    public ushort serverPort = 7777; // 로비에서 받은 서버 포트

    void Start()
    {
        // 서버에 연결
        ConnectToServer(serverIp, serverPort);
    }

    private void ConnectToServer(string ip, ushort port)
    {
        NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().SetConnectionData(ip, port);
        NetworkManager.Singleton.StartClient();
        Debug.Log("서버에 연결 시도 중...");
    }

    private void OnClientConnected(ulong clientId)
    {
        // 연결 성공 시 처리
        Debug.Log($"클라이언트 {clientId}가 서버에 연결됨");
    }
}