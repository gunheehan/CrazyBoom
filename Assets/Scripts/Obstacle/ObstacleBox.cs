using UnityEngine;

public class ObstacleBox : MonoBehaviour, IObstacle
{
    private int boxHP;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxHP = Random.Range(1, 3);
    }

    public void Damage()
    {
        boxHP--;
        Debug.Log("Box HP : " + boxHP);
        if(boxHP < 1)
            gameObject.SetActive(false);
    }
}
