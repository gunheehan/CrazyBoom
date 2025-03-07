using UnityEngine;

public class PlaneParts : MonoBehaviour
{
    [SerializeField] private ObstacleBox obstacleItem;
    [SerializeField] private WaterBomb bomb;

    public void SetPlane(Vector3 position, bool isneedObstacle)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = position;
        if (isneedObstacle)
            SetObstacleBox();
    }

    public void SetBomb(int power)
    {
        bomb.SetBomb(power);
    }

    public void SetObstacleBox()
    {
        obstacleItem.SetObstacleBox();
    }
}
