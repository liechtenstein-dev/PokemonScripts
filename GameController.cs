using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerControlMovement player;
    [SerializeField] BattleSystem  battleSystem;
    [SerializeField] private Camera worldCamera;
    private GameState state;

    private void Start()
    {
        player.onEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    private void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        player.ResetConfiguration();
    }
    
    private void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        battleSystem.StartBattle();
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            player.HandleUpdate();   
        }
        else if (state  == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
