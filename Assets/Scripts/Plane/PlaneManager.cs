using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour
{
    public event Action OnCompletedPlaneSetting = null;
    
    [SerializeField] private PlaneParts plane;
    [SerializeField] private GameObject obstacleContents;

    private HashSet<Vector2Int> ignorefloorpos;
    private readonly int MAPSIZE = 10;
    private List<Vector2> playerPos;
    
    private void Start()
    {
        ignorefloorpos = new HashSet<Vector2Int>();
        playerPos = new List<Vector2>();
        AddEdgeIgnoreArea(0, 0);
        AddEdgeIgnoreArea(0, MAPSIZE - 3);
        AddEdgeIgnoreArea(MAPSIZE - 3, 0);
        AddEdgeIgnoreArea(MAPSIZE - 3, MAPSIZE - 3);
        
        CreateObstacleBox();
    }

    public Vector3 GetPlayerInitPos(int playerNumber)
    {
        if (playerNumber > playerPos.Count)
            return Vector3.zero;
        
        Vector2 playerpos = playerPos[playerNumber];
        // Vector3 localPosition = obstacleContents.transform.InverseTransformPoint(new Vector3(playerpos.x, 1.5f, playerpos.y)); // Start World Pos to Plane LocalPos

        return new Vector3(playerpos.x, 1.5f, playerpos.y);
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
            if (IsValidPosition(nx, ny))
                ignorefloorpos.Add(new Vector2Int(nx, ny));
            if (offset.x == 1 && offset.y == 1)
            {
                Renderer prefabRenderer = plane.GetComponent<Renderer>();
                float prefabSizeX = prefabRenderer.bounds.size.x;
                float prefabSizeZ = prefabRenderer.bounds.size.z;
                
                float worldX = nx * prefabSizeX;
                float worldZ = ny * prefabSizeZ;

                playerPos.Add(new Vector2(worldX, -worldZ));
            }
        }
    }
    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < MAPSIZE && y >= 0 && y < MAPSIZE;
    }
    
    public void CreateObstacleBox()
    {
        Renderer prefabRenderer = plane.GetComponent<Renderer>();
        float prefabSizeX = prefabRenderer.bounds.size.x;
        float prefabSizeZ = prefabRenderer.bounds.size.z;
        
        for (int x = 0; x < MAPSIZE; x++)
        {
            for (int z = 0; z < MAPSIZE; z++)
            {
                Vector2Int initpos = new Vector2Int(x, z);
                Vector3 position = new Vector3(
                    (x * prefabSizeX),
                    1,
                    -(z * prefabSizeZ)
                );
                
                if (ignorefloorpos.Contains(initpos))
                {
                    PlaneParts obstacleObject = Instantiate(plane, obstacleContents.transform);
                    obstacleObject.transform.SetAsLastSibling();
                    obstacleObject.SetPlane(position, false);
                    continue;
                }

                SetObstacleBox(position);
            }
        }
        
        OnCompletedPlaneSetting?.Invoke();
    }

    private void SetObstacleBox(Vector3 position)
    {
        PlaneParts obstacleObject = Instantiate(plane, obstacleContents.transform);

        obstacleObject.transform.SetAsLastSibling();
        obstacleObject.SetPlane(position, true);
    }
}
