using UnityEngine;

public class PlaneParts : MonoBehaviour
{
    [SerializeField] private ObstacleBox obstacleItem;

    public void SetPlane(Vector3 position, bool isneedObstacle)
    {
        gameObject.transform.position = position;
        if(isneedObstacle)
            obstacleItem.gameObject.SetActive(true);
    }
}
