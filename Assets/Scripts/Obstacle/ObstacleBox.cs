using System;
using Unity.Netcode;
using UnityEngine;

public class ObstacleBox : NetworkBehaviour, IObstacle
{
    public event Action OnDestroyBox = null; 
    private NetworkVariable<int> boxHp = new NetworkVariable<int>(
        writePerm: NetworkVariableWritePermission.Server
    );

    private NetworkVariable<bool> isActive = new NetworkVariable<bool>(
        writePerm: NetworkVariableWritePermission.Server
    );
    
    public void SetInitialHp(int hp)
    {
        Debug.Log("SetObstacleBox");
        if (!NetworkManager.Singleton.IsServer) return;

        boxHp.Value = hp;
        isActive.Value = true;
        gameObject.SetActive(isActive.Value);
    }

    public void Damage()
    {
        boxHp.Value--;
        if (boxHp.Value < 1)
        {
            OnDestroyBox?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
