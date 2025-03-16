using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem waterBombParticle;
    [SerializeField] private Transform particleContents;

    private Stack<ParticleSystem> particlePool = new Stack<ParticleSystem>();

    public void CreateBombParticle(Vector3 startPos, Vector3 endPos)
    {
        ParticleSystem splash;

        if (particlePool.Count > 0)
            splash = particlePool.Pop();
        else
            splash = Instantiate(waterBombParticle, particleContents);

        Vector3 direction = (endPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, endPos);

        splash.transform.position = startPos;
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
        particlePool.Push(splash);
    }
}
