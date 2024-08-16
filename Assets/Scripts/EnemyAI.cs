using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnEnded += TurnSystem_OnTurnEnded;
    }
    private void Update()
    {
        if (TurnSystem.Instance.GetIsPlayerTurn()) return;


        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if(TryTakeEnemyAIAction(SetStateToTakingTurn))
                         state = State.Busy;
                    //state = State.WaitingForEnemyTurn;
                    else      
                         TurnSystem.Instance.NextTurn();
                    state = State.WaitingForEnemyTurn;
                }
                break;
            case State.Busy:
                break;

        }
    }

    private void SetStateToTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }


    private void TurnSystem_OnTurnEnded(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.GetIsPlayerTurn())
        {
            state = State.TakingTurn; 
            timer = 2.3f; //temp value
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        //Debug.Log("Try Take an Action");
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if(TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))   
                return true;
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.HaveEnoughActionPointsToTakeAction(baseAction)) continue; //not enough AP to take such action

            if (bestEnemyAIAction == null)
            { 
                bestEnemyAIAction =  baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else 
            return false;
        
    }

}
