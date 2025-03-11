using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    // [SerializeField] private WaterBomb bombPrefab;
    //
    // private Stack<WaterBomb> bombStack;
    // private int power = 1;
    //
    // private void Start()
    // {
    //     bombStack = new Stack<WaterBomb>();
    // }
    //
    // private void OnDestroy()
    // {
    //     foreach (var bomb in bombStack)
    //     {
    //         bomb.OnExplosionEvent -= OnExplosion;
    //     }
    // }
    //
    // public void OnUpdatePower(int newPower)
    // {
    //     power = newPower;
    // }
    //
    // public void SetBomb(Vector3 position)
    // {
    //     WaterBomb bomb = GetBomb();
    //     bomb.gameObject.transform.position = position;
    //     bomb.SetBomb(power);
    // }
    //
    // private WaterBomb GetBomb()
    // {
    //     if (bombStack.Count > 0)
    //         return bombStack.Pop();
    //
    //     WaterBomb newbomb = GameObject.Instantiate(bombPrefab.gameObject).GetComponent<WaterBomb>();
    //     newbomb.OnExplosionEvent += OnExplosion;
    //     return newbomb;
    // }
    //
    // private void OnExplosion(WaterBomb bomb)
    // {
    //     bombStack.Push(bomb);
    // }
}
