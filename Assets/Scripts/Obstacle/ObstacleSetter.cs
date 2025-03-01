using UnityEngine;

public class ObstacleSetter : MonoBehaviour
{
    private void Start()
    {
        CreateObstacleBox();
    }

    [SerializeField] private GameObject floorContents;
    [SerializeField] private GameObject floorPrefab;

    private readonly int MAPSIZE = 10;
    
    public void CreateObstacleBox()
    {
        Vector3 startPosition = Vector3.zero;
        
        for (int x = 0; x < MAPSIZE; x++)
        {
            for (int z = 0; z < MAPSIZE; z++)
            {
                Vector3 position = startPosition + new Vector3(x, 1, z);
                SetObstacleBox(position);
            }
        } 
    }

    private void SetObstacleBox(Vector3 position)
    {
        GameObject obstacleObject = Instantiate(floorPrefab, floorContents.transform);
        obstacleObject.transform.position = position + new Vector3(position.x, 0, position.z);

        obstacleObject.name = "ObstacleBox " + position.x + " / " + position.z;

        obstacleObject.GetComponent<Renderer>().material.color = Color.blue;

        obstacleObject.transform.SetAsLastSibling();
        obstacleObject.SetActive(true);
    }
}
