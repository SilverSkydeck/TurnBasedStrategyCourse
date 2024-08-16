using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{

    [SerializeField] private float totalSpinAmount;

    private void Update()
    {
        if (!isUnitActive) return;


        float spinAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAmount, 0);
        totalSpinAmount += spinAmount;

        if (totalSpinAmount >= 360f) StopSpin();

    }


    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        totalSpinAmount = 0f;

        ActionStart(onActionComplete);

    }

    public override List<GridPosition> GetValidGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition>
        {
            unitGridPosition
        };

    }

    public void StopSpin()
    {

        totalSpinAmount = 0f;
        ActionComplete(onActionComplete);

    }

    public override string GetActionName()
    {
        return "D.Spin";
    }

    public override int GetActionPointCost()
    {
        return 1;
    }



    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }
}
