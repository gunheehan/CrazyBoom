using System;
using Unity.Netcode;

public class ObstacleBox : NetworkBehaviour, IObstacle
{
    public event Action OnDestroyBox = null; 
    private NetworkVariable<int> boxHp = new NetworkVariable<int>(
        writePerm: NetworkVariableWritePermission.Server
    );
    
    public void SetInitialHp(int hp)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        boxHp.Value = hp;
        gameObject.SetActive(true);
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
