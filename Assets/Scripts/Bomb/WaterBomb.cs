using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class WaterBomb : NetworkBehaviour
{
    public event Action<Vector3, Vector3> OnExplodeDirectionAction;
    public event Action<string> ExplodeBomb;
    [SerializeField] private Collider collider;
    
    private int objIndex;
    private string bombOwner;
    
    private int explosionRange = 1;
    private LayerMask obstacleLayer;
    private LayerMask playerLayer;
    private LayerMask bombLayer;
    private LayerMask buffLayer;

    private Collider playerCollider;

    private bool isSet = false;
    public bool IsSet => isSet;
    private bool explode = false;
    
    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player", "OtherPlayer");
        obstacleLayer = LayerMask.GetMask("Obstacle"); 
        bombLayer = LayerMask.GetMask("Bomb"); 
        buffLayer = LayerMask.GetMask("BuffItem"); 
    }
    
    private void Update()
    {
        if (playerCollider != null && collider != null)
        {
            Bounds expandedBounds = collider.bounds;
            expandedBounds.Expand(1.5f);

            if (!expandedBounds.Contains(playerCollider.bounds.min) || 
                !expandedBounds.Contains(playerCollider.bounds.max))
            {
                collider.isTrigger = false;
            }
        }
    }

    public void SetBomb(int power, string bombowner)
    {
        if (isSet)
            return;

        isSet = false;
        collider.isTrigger = true;
        bombOwner = bombowner;
        gameObject.SetActive(true);
        explosionRange = power;
        StartCoroutine(WaitExplode());
    }

    public void Explode()
    {
        if (explode)
            return;

        explode = true;
        StopCoroutine(WaitExplode());
        
        Vector3 origin = transform.position;
        CheckDirection(Vector3.right, origin, explosionRange);
        CheckDirection(Vector3.left, origin, explosionRange);
        CheckDirection(Vector3.forward, origin, explosionRange);
        CheckDirection(Vector3.back, origin, explosionRange);
        
        ExplodeBomb?.Invoke(bombOwner);
        BombReset();
    }

    IEnumerator WaitExplode()
    {
        objIndex = 0;
        yield return new WaitForSeconds(2f);

        Explode();
    }

    private void BombReset()
    {
        isSet = false;
        explode = false;
        bombOwner = string.Empty;
        gameObject.SetActive(false);
    }

    private void CheckDirection(Vector3 direction, Vector3 origin, float remainingDistance)
    {
        if (remainingDistance <= 0) return;

        RaycastHit hit;
        int layerMask = playerLayer | obstacleLayer | bombLayer | buffLayer;
        Vector3 neworigin = new Vector3(origin.x, 2f, origin.z);

        if (Physics.Raycast(neworigin, direction, out hit, remainingDistance, layerMask))
        {
            int hitLayer = hit.collider.gameObject.layer;
            float newRemainingDistance = remainingDistance - hit.distance;
            if (((1 << hitLayer) & buffLayer) != 0)
            {
                IBuff item = hit.collider.gameObject.GetComponent<IBuff>();
                item?.TakeDamege();
                CheckDirection(direction, hit.point + direction.normalized * 0.1f, newRemainingDistance);
            }
            if (((1 << hitLayer) & bombLayer) != 0)
            {
                WaterBomb bomb = hit.collider.gameObject.GetComponent<WaterBomb>();
                bomb.Explode();
            }
            if (((1 << hitLayer) & playerLayer) != 0)
            {
                IPlayer player = hit.collider.gameObject.GetComponent<IPlayer>();
                player?.TakeDamage();
                CheckDirection(direction, hit.point + direction.normalized * 0.1f, newRemainingDistance);
            }
            
            if (((1 << hitLayer) & obstacleLayer) != 0)
            {
                IObstacle obstacle = hit.collider.gameObject.GetComponent<IObstacle>();
                obstacle?.Damage();
                CreateParticleEffect(hit.point + direction.normalized * (hit.distance - 0.1f));
            }
        }
        else
        {
            CreateParticleEffect(origin + direction.normalized * remainingDistance);
        }
    }


    private void CreateParticleEffect(Vector3 position)
    {
        OnExplodeDirectionAction?.Invoke(gameObject.transform.position, position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerCollider = other.gameObject.GetComponent<Collider>();
        }
    }
}
