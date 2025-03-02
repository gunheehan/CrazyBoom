using UnityEngine;

public class ObstacleSetter : MonoBehaviour
{
    private void Start()
    {
        CreateObstacleBox();
    }
    [SerializeField] private GameObject floorObject;
    [SerializeField] private GameObject obstacleContents;
    [SerializeField] private GameObject obstaclePrefab;

    private readonly int MAPSIZE = 10;
    
    public void CreateObstacleBox()
    {
        Renderer planeRenderer = floorObject.GetComponent<Renderer>();
        float planeSizeX = planeRenderer.bounds.size.x;
        float planeSizeZ = planeRenderer.bounds.size.z;

        Renderer prefabRenderer = obstaclePrefab.GetComponent<Renderer>();
        float prefabSizeX = prefabRenderer.bounds.size.x;
        float prefabSizeZ = prefabRenderer.bounds.size.z;
        
        float startX = - (planeSizeX / 2) + (prefabSizeX / 2);
        float startZ = (planeSizeZ / 2) - (prefabSizeZ / 2);
        
        Vector3 localPosition = floorObject.transform.InverseTransformPoint(new Vector3(startX, 0 ,startZ)); // Start World Pos to Plane LocalPos

        for (int x = 0; x < MAPSIZE; x++)
        {
            for (int z = 0; z < MAPSIZE; z++)
            {
                Vector3 position = new Vector3(
                    localPosition.x + (x * prefabSizeX),
                    1,
                    localPosition.z - (z * prefabSizeZ)
                );
                SetObstacleBox(position);
            }
        }
    }

    private void SetObstacleBox(Vector3 position)
    {
        GameObject obstacleObject = Instantiate(obstaclePrefab, obstacleContents.transform);
        obstacleObject.transform.position = position;
        obstacleObject.name = "ObstacleBox " + position.x + " / " + position.z;

        obstacleObject.GetComponent<Renderer>().material.color = Color.blue;

        obstacleObject.transform.SetAsLastSibling();
        obstacleObject.SetActive(true);
    }
}
