using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<PlayerManager> player;

    [SerializeField] private PlaneManager planeManager;

    private void OnEnable()
    {
       //planeManager.OnCompletedPlaneSetting += OnCompletedPlane;
    }

    private void OnDisable()
    {
      //  planeManager.OnCompletedPlaneSetting -= OnCompletedPlane;
    }

    private void OnCompletedPlane()
    {
        SetPlayer();
    }

    private void SetPlayer()
    {
        Debug.Log("Player Set");
        for (int index = 0; index < player.Count; index++)
        {
            Vector3 playerpos = planeManager.GetPlayerInitPos(index);
            player[index].InitPlayer(playerpos);
        }
    }
}
