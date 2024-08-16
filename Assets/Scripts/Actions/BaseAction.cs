
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected Unit unit;
    protected bool isUnitActive;

    protected Action onActionComplete;

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);


    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidGridPositionList();

    public virtual int GetActionPointCost()
    {
        return 0;
    }

    protected void ActionStart(Action onActionComplete)
    {
        isUnitActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete(Action onActionComplete)
    {
        isUnitActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> gridPositionList = GetValidGridPositionList();

        foreach (GridPosition gridPosition in gridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);

        }

        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort(
                (EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue
                );

            return enemyAIActionList[0];
        }
        else //no possible ai actions
            return null;

    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);


}
