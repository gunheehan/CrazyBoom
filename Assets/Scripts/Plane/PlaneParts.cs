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

    private NetworkVariable<Vector3> planePosition = new NetworkVariable<Vector3>();
    private NetworkVariable<int> obstacleHP = new NetworkVariable<int>();
    private NetworkVariable<int> buffItemIndex = new NetworkVariable<int>();

    public Action<string> explodeBomb;
    private void OnEnable()
    {
        obstacleItem.OnDestroyBox += SetBuffItem;
        bomb.OnExplodeDirectionAction += particleController.CreateBombParticle;
        bomb.ExplodeBomb += OnExplodeBomb;
    }

    private void OnDisable()
    {
        obstacleItem.OnDestroyBox -= SetBuffItem;
        bomb.OnExplodeDirectionAction -= particleController.CreateBombParticle;
        bomb.ExplodeBomb -= OnExplodeBomb;
    }

    public override void OnNetworkSpawn()
    {
        SetPosition(planePosition.Value);
        SetObstacleObject(obstacleHP.Value);

        planePosition.OnValueChanged += (_, newValue) =>
        {
            SetPosition(newValue);
        };

        obstacleHP.OnValueChanged += (_, newValue) =>
        {
            SetObstacleObject(newValue);
        };
    }

    public void Init(Vector3 pos, bool activateChild)
    {
        if (!IsServer) return;

        planePosition.Value = pos;

        if (activateChild)
        {
            obstacleHP.Value = Random.Range(1, 3);
            buffItemIndex.Value = Random.Range(0, 8);
        }
    }

    private void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    private void SetObstacleObject(int hp)
    {
        if (hp < 1)
            return;
        
        obstacleItem.SetInitialHp(hp);
        obstacleItem.gameObject.SetActive(true);
    }

    private void SetBuffItem()
    {
        buffItem.SetBuffItem(buffItemIndex.Value);
    }

    private void OnExplodeBomb(string owner)
    {
        explodeBomb?.Invoke(owner);
        explodeBomb = null;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SetBombServerRpc(int power, string playerID)
    {
        if (bomb.IsSet)
            return;

        SetBombClientRpc(power, playerID);
    }
    
    [ClientRpc]
    private void SetBombClientRpc(int power, string playerID)
    {
        bomb.SetBomb(power, playerID);
    }
}
