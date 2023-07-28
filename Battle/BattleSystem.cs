using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
        Start,
        PlayerAction,
        PlayerMove,
        EnemyMove,
        Busy
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;

    [SerializeField] private BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;
    
    private BattleState state;
    private int currentAction;
    private int currentMove;
    
    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
        
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        yield return dialogBox.TypeDialog($"Un {enemyUnit.Pokemon.Base.Name} salvaje aparecio.");
        yield return new WaitForSeconds(1f);
        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Selecciona una accion"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);
        currentAction = 0;
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    { 
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
            {
                ++currentAction;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                --currentAction;
            }
        }
      
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            var action = currentAction;
            currentAction = -1;
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(false);
            PlayerMove();
        }
        dialogBox.UpdateActionSelection(currentAction);
    }

    IEnumerator PlayerRunMove(Move move)
    {   
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} uso {move.Base.Name}");
        playerUnit.PlayAttackAnimation();
        yield return  new WaitForSeconds(1f);
        enemyUnit.PlayHitAnimation();
        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        if(damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} fue derrotado");
            enemyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        var move = enemyUnit.Pokemon.GetRandomMove();
       
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} uso {move.Base.Name}");
        
        enemyUnit.PlayAttackAnimation();
        yield return  new WaitForSeconds(1f);
        
        playerUnit.PlayHitAnimation();
        var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
       
        yield return playerHud.UpdateHP();
       
        yield return ShowDamageDetails(damageDetails);
      
        if(damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} fue derrotado");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("¡El ataque fue crítico!");
        }
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("¡El ataque fue super eficaz!");
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("¡El ataque no fue muy eficaz!");
        }
    }
    
    
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
            {
                ++currentMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                --currentMove;
            }
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
            {
                currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                currentMove -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Pokemon.Moves[currentMove];
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerRunMove(move));
        }
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);
    }
}
