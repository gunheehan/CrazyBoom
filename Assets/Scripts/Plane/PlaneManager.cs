using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlaneManager : NetworkBehaviour
{
    public event Action OnCompletedPlaneSetting = null;
    
    [SerializeField] private PlaneParts plane;
    [SerializeField] private GameObject obstacleContents;
    [SerializeField] private GameObject blockWall;
    [SerializeField] private GameObject blockWallContents;

    private HashSet<Vector2Int> ignorefloorpos;
    private readonly int MAPSIZE = 10;
    private List<Vector2> playerPos;
    
    private void Awake()
    {
        ignorefloorpos = new HashSet<Vector2Int>();
        playerPos = new List<Vector2>();
    }

    private void OnEnable()
    {
        GameRoomManager.Instance.OnStartGameProcess += OnSetPlane;
    }

    private void OnDisable()
    {
        GameRoomManager.Instance.OnStartGameProcess += OnSetPlane;
    }

    private void OnSetPlane(bool isconnect)
    {
        Debug.Log("Set Plane");
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
        if (!IsServer) return;
Debug.Log("IsServer Plane Setting Start");
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

                PlaneParts obstacleObject = NetworkManager.Instantiate(plane, obstacleContents.transform);
                //obstacleObject.transform.SetAsLastSibling();
                if (ignorefloorpos.Contains(initpos))
                    obstacleObject.SetPlane(position, false);
                else
                    obstacleObject.SetPlane(position, true);
            }
        }

        CreateBoundaryWalls();
        OnCompletedPlaneSetting?.Invoke();
    }
    
    public void CreateBoundaryWalls()
    {
        Renderer prefabRenderer = plane.GetComponent<Renderer>();
        float prefabSizeX = prefabRenderer.bounds.size.x;
        float prefabSizeZ = prefabRenderer.bounds.size.z;

        for (int x = -1; x <= MAPSIZE; x++) // 바운더리 포함
        {
            for (int z = -1; z <= MAPSIZE; z++) // 바운더리 포함
            {
                Vector3 position = new Vector3(
                    (x * prefabSizeX),
                    2,
                    -(z * prefabSizeZ)
                );

                if (x == -1 || x == MAPSIZE || z == -1 || z == MAPSIZE)
                {
                    Quaternion rotation = Quaternion.identity;

                    // 행 방향 (위쪽/아래쪽)에 위치한 블록이면 Y축 90도 회전 적용
                    if (z == -1 || z == MAPSIZE)
                    {
                        rotation = Quaternion.Euler(0, 90, 0);
                    }

                    Instantiate(blockWall, position, rotation, blockWallContents.transform);
                }
            }
        }
    }
}
