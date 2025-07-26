using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlaneParts : NetworkBehaviour
{
    [SerializeField] private ObstacleBox obstacleItem;
    [SerializeField] private WaterBomb bomb;
    [SerializeField] private BuffItem buffItem;
    [SerializeField] private ParticleController particleController;

    private void OnEnable()
    {
        obstacleItem.OnDestroyBox += buffItem.SetBuffItem;
        bomb.OnExplodeDirectionAction += particleController.CreateBombParticle;
    }

    private void OnDisable()
    {
        obstacleItem.OnDestroyBox -= buffItem.SetBuffItem;
        bomb.OnExplodeDirectionAction -= particleController.CreateBombParticle;
    }

    // public void SetPlane(Vector3 position, bool isneedObstacle)
    // {
    //     Debug.Log("Plane Obstacle State : " + isneedObstacle);
    //     gameObject.SetActive(true);
    //     gameObject.transform.position = position;
    //     if (isneedObstacle)
    //         SetObstacleBox();
    // }
    
    [ServerRpc(RequireOwnership = false)]
    public void SetBombServerRpc(int power, string playerID)
    {
        bomb.SetBomb(power, playerID);
    }

    private void SetObstacleBox()
    {
        obstacleItem.SetInitialHp(Random.Range(1, 3)); // Init 값 설정
    }
    
    private NetworkVariable<Vector3> planePosition = new NetworkVariable<Vector3>();
    private NetworkVariable<bool> isActivated = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        Debug.Log("Network Spawn");
        // 값이 동기화되었을 때 로컬에서 처리
        planePosition.OnValueChanged += (_, newValue) =>
        {
            SetPlane(newValue, isActivated.Value);
        };

        isActivated.OnValueChanged += (_, newValue) =>
        {
            SetPlane(planePosition.Value, newValue);
        };

        SetPlane(planePosition.Value, isActivated.Value);
    }

    public void Init(Vector3 pos, bool activateChild)
    {
        if (!IsServer) return;

        planePosition.Value = pos;
        isActivated.Value = activateChild;
    }

    private void SetPlane(Vector3 pos, bool activateChild)
    {
        Debug.Log("set Plane : " + activateChild);

        transform.localPosition = pos;

        if (activateChild)
        {
            obstacleItem.SetInitialHp(Random.Range(1, 3)); // Init 값 설정
            obstacleItem.gameObject.SetActive(true);
        }
    }
}
