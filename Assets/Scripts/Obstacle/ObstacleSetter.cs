using System.Collections.Generic;
using UnityEngine;

public class ObstacleSetter : MonoBehaviour
{
    [SerializeField] private GameObject floorObject;
    [SerializeField] private GameObject obstacleContents;
    [SerializeField] private GameObject obstaclePrefab;

    private HashSet<Vector2Int> ignorefloorpos;
    private readonly int MAPSIZE = 10;
    
    private void Start()
    {
        ignorefloorpos = new HashSet<Vector2Int>();
        AddEdgeIgnoreArea(0, 0);
        AddEdgeIgnoreArea(0, MAPSIZE - 3);
        AddEdgeIgnoreArea(MAPSIZE - 3, 0);
        AddEdgeIgnoreArea(MAPSIZE - 3, MAPSIZE - 3);
        
        CreateObstacleBox();
    }

    // Player 생성 위치 생성 무시(+모양)
    private void AddEdgeIgnoreArea(int x, int y)
    {
        List<Vector2Int> offsets = new List<Vector2Int>
        {
            new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(1,1),
            new Vector2Int(1,2), new Vector2Int(2,1)
        };

        foreach (var offset in offsets)
        {
            int nx = x + offset.x;
            int ny = y + offset.y;
            Debug.Log(nx + "/" + ny);
            if (IsValidPosition(nx, ny))
                ignorefloorpos.Add(new Vector2Int(nx, ny));
        }
    }
    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < MAPSIZE && y >= 0 && y < MAPSIZE;
    }
    
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
                Vector2Int initpos = new Vector2Int(x, z);
                if (ignorefloorpos.Contains(initpos))
                    continue;
                
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
