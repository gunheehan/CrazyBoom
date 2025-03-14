using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBomb : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private ParticleSystem particle;

    private Stack<ParticleSystem> pool = new Stack<ParticleSystem>();

    private Action OnExplodeAction;

    private int objIndex;
    
    private int explosionRange = 1;
    private LayerMask obstacleLayer;
    private LayerMask playerLayer;
    private LayerMask bombLayer;
    private LayerMask buffLayer;

    private Collider playerCollider;

    private bool isSet = false;
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

    public void SetBomb(int power, Action OnExplode)
    {
        if (isSet)
            return;

        isSet = false;
        collider.isTrigger = true;
        OnExplodeAction = OnExplode;
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
        
        OnExplodeAction?.Invoke();
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
        CreateSplash(gameObject.transform.position, position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerCollider = other.gameObject.GetComponent<Collider>();
        }
    }
    
    private void CreateSplash(Vector3 start, Vector3 end)
    {
        ParticleSystem splash;

        if (pool.Count > 0)
            splash = pool.Pop();
        else
            splash = Instantiate(particle);

        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        splash.transform.position = start;
        splash.transform.rotation = Quaternion.LookRotation(direction);

        var main = splash.main;
        main.startSpeed = distance * 5f;

        splash.Emit(30);
        splash.gameObject.SetActive(true);

        StartCoroutine(ReturnToPool(splash));
    }

    private IEnumerator ReturnToPool(ParticleSystem splash)
    {
        yield return new WaitForSeconds(1f);
        splash.gameObject.SetActive(false);
        pool.Push(splash);
    }
}
