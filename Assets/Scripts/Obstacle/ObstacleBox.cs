using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleBox : NetworkBehaviour, IObstacle
{
    public event Action OnDestroyBox = null; 
    private int boxHP;

    public void SetObstacleBox()
    {
        if (!IsServer)
        {
            Debug.Log("Not server");
            return;
        }
        
        if (!IsHost)
        {
            Debug.Log($"[{gameObject.name}] IsOwner: {IsOwner}, IsServer: {IsServer}, IsClient: {IsClient}, OwnerClientId: {OwnerClientId}, LocalClientId: {NetworkManager.Singleton.LocalClientId}");
            Debug.Log("Obstacle Set Not Access");
            return;
        }
        
        int hp = Random.Range(1, 3);
        HandleCreateObstacleServerRpc(hp);
    }

    [ServerRpc]
    private void HandleCreateObstacleServerRpc(int hp)
    {
        boxHP = hp;
        gameObject.SetActive(true);
    }

    public void Damage()
    {
        boxHP--;
        if (boxHP < 1)
        {
            OnDestroyBox?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
