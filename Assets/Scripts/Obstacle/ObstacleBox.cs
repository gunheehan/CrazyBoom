using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleBox : MonoBehaviour, IObstacle
{
    public event Action OnDestroyBox = null; 
    private int boxHP;

    public void SetObstacleBox()
    {
        boxHP = Random.Range(1, 3);
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
