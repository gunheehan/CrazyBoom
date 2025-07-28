using System;
using Unity.Netcode;
using UnityEngine;

public class ObstacleBox : NetworkBehaviour, IObstacle
{
    public event Action OnDestroyBox = null;
    private int boxHp;
    
    public void SetInitialHp(int hp)
    {
        boxHp = hp;
    }

    public void Damage()
    {
        boxHp--;
        if (boxHp < 1)
        {
            OnDestroyBox?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
