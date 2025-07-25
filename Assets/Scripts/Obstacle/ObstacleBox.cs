using System;
using Unity.Netcode;
using UnityEngine;

public class ObstacleBox : MonoBehaviour, IObstacle
{
    public event Action OnDestroyBox = null; 
    private NetworkVariable<int> boxHp = new NetworkVariable<int>(
        writePerm: NetworkVariableWritePermission.Server
    );
    
    public void SetInitialHp(int hp)
    {
        Debug.Log("SetObstacleBox");
        gameObject.SetActive(true);
        if (!NetworkManager.Singleton.IsServer) return;

        boxHp.Value = hp;
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
