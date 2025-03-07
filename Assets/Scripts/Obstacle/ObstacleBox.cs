using UnityEngine;

public class ObstacleBox : MonoBehaviour, IObstacle
{
    private int boxHP;

    public void SetObstacleBox()
    {
        boxHP = Random.Range(1, 3);
        gameObject.SetActive(true);
    }

    public void Damage()
    {
        boxHP--;
        if(boxHP < 1)
            gameObject.SetActive(false);
    }
}
