using System;
using Unity.Netcode;
using UnityEngine;

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

    public void SetPlane(Vector3 position, bool isneedObstacle)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = position;
        if (isneedObstacle)
            SetObstacleBox();
    }

    public void SetBomb(int power, Action OnExplodeCallback)
    {
        bomb.SetBomb(power, OnExplodeCallback);
    }

    public void SetObstacleBox()
    {
        obstacleItem.SetObstacleBox();
    }
}
