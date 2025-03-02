using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombController : MonoBehaviour
{
    [SerializeField] private WaterBomb bombPrefab;
    [SerializeField] private Button BombButton;
    [SerializeField] private GameObject player;
    
    private PlayerController controller;

    private Stack<WaterBomb> bombStack;
    
    private void Start()
    {
        bombStack = new Stack<WaterBomb>();
        BombButton.onClick.AddListener(OnClickBomb);
    }

    private void OnEnable()
    {
        if (controller == null)
            controller = new PlayerController();
        controller.Player.Attack.performed += context => OnClickBomb();
        controller.Player.Attack.Enable();
    }

    private void OnDisable()
    {
        controller.Player.Attack.performed -= context => OnClickBomb();
        controller.Player.Attack.Disable();
    }

    private void OnDestroy()
    {
        foreach (var bomb in bombStack)
        {
            bomb.OnExplosionEvent -= OnExplosion;
        }
    }

    private void OnClickBomb()
    {
        WaterBomb bomb = GetBomb();
        bomb.gameObject.transform.position = player.transform.position;
        bomb.SetBomb();
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
