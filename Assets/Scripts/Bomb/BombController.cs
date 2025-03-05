using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class BombController : MonoBehaviour
{
    [SerializeField] private WaterBomb bombPrefab;
    [SerializeField] private Button BombButton;
    [SerializeField] private GameObject player;
    
    private Stack<WaterBomb> bombStack;
    private int power = 1;
    
    private void Start()
    {
        bombStack = new Stack<WaterBomb>();
        BombButton.onClick.AddListener(OnClickBomb);
    }

    private void OnDestroy()
    {
        foreach (var bomb in bombStack)
        {
            bomb.OnExplosionEvent -= OnExplosion;
        }
    }

    public void OnUpdatePower(int newPower)
    {
        power = newPower;
    }

    private void OnClickBomb()
    {
        // WaterBomb bomb = GetBomb();
        // bomb.gameObject.transform.position = player.transform.position;
        // bomb.SetBomb(power);

        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(Key.Space));
    }

    private WaterBomb GetBomb()
    {
        if (bombStack.Count > 0)
            return bombStack.Pop();

        WaterBomb newbomb = GameObject.Instantiate(bombPrefab.gameObject).GetComponent<WaterBomb>();
        newbomb.OnExplosionEvent += OnExplosion;
        return newbomb;
    }

    private void OnExplosion(WaterBomb bomb)
    {
        bomb.gameObject.SetActive(false);
        bombStack.Push(bomb);
    }
}
