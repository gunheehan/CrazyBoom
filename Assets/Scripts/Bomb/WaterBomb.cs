using System;
using System.Collections;
using UnityEngine;

public class WaterBomb : MonoBehaviour
{
    public event Action<WaterBomb> OnExplosionEvent = null;
    private int objIndex;
    
    private int explosionRange = 1;
    private LayerMask obstacleLayer;
    private LayerMask playerLayer;
    private LayerMask bomb;

    private bool isSet = false;
    private bool explode = false;
    
    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player", "OtherPlayer");
        obstacleLayer = LayerMask.GetMask("Obstacle"); 
        bomb = LayerMask.GetMask("Bomb"); 
    }

    public void SetBomb(int power)
    {
        if (isSet)
            return;

        isSet = false;
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
        
        OnExplosionEvent?.Invoke(this);
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
        int layerMask = playerLayer | obstacleLayer | bomb;

        if (Physics.Raycast(origin, direction, out hit, remainingDistance, layerMask))
        {
            int hitLayer = hit.collider.gameObject.layer;
            float newRemainingDistance = remainingDistance - hit.distance;

            if (((1 << hitLayer) & bomb) != 0)
            {
                WaterBomb bomb = hit.collider.gameObject.GetComponent<WaterBomb>();
                bomb.Explode();
                CheckDirection(direction, hit.point + direction.normalized * 0.1f, newRemainingDistance);
            }
            else if (((1 << hitLayer) & playerLayer) != 0)
            {
                CheckDirection(direction, hit.point + direction.normalized * 0.1f, newRemainingDistance);
            }
            else if (((1 << hitLayer) & obstacleLayer) != 0)
            {
                IObstacle obstacle = hit.collider.gameObject.GetComponent<IObstacle>();
                obstacle.Damage();
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
        // obj[objIndex].transform.position = new Vector3(position.x, gameObject.transform.position.y, position.z);
        // objIndex++;
    }
}
