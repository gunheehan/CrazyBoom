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
    
    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player", "OtherPlayer");
        obstacleLayer = LayerMask.GetMask("Obstacle"); 
    }

    public void SetBomb(int power)
    {
        gameObject.SetActive(true);
        explosionRange = power;
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        Debug.Log("Bomb 대기");
        objIndex = 0;
        yield return new WaitForSeconds(2f);
        
        Vector3 origin = transform.position;
        CheckDirection(Vector3.right, origin);
        CheckDirection(Vector3.left, origin);
        CheckDirection(Vector3.forward, origin);
        CheckDirection(Vector3.back, origin);
        
        OnExplosionEvent?.Invoke(this);
    }

    private void CheckDirection(Vector3 direction, Vector3 origin)
    {
        RaycastHit hit;

        int layerMask = playerLayer | obstacleLayer;

        if (Physics.Raycast(origin, direction, out hit, explosionRange, layerMask))
        {
            int hitLayer = hit.collider.gameObject.layer;

            if (((1 << hitLayer) & playerLayer) != 0)
            {
                float remainingDistance = explosionRange - hit.distance;

                RaycastHit secondHit;
                if (Physics.Raycast(hit.point + direction.normalized * 0.1f, direction, out secondHit, remainingDistance, obstacleLayer))
                {
                    int secondHitLayer = secondHit.collider.gameObject.layer;
                    if (((1 << secondHitLayer) & obstacleLayer) != 0)
                    {
                        IObstacle obstacle = secondHit.collider.gameObject.GetComponent<IObstacle>();
                        obstacle.Damage();
                        CreateParticleEffect(secondHit.point + direction.normalized * (secondHit.distance - 0.1f));
                    }
                }
                else
                {
                    CreateParticleEffect(hit.point + direction.normalized * (hit.distance - 0.1f));
                }
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
            CreateParticleEffect(origin + direction.normalized * explosionRange);
        }
    }

    private void CreateParticleEffect(Vector3 position)
    {
        // obj[objIndex].transform.position = new Vector3(position.x, gameObject.transform.position.y, position.z);
        // objIndex++;
    }
}
