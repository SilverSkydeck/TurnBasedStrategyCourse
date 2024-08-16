using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    private int maxMoveDistance = 5 ;

    //private Vector3 targetPosition;
    private List<Vector3> positionList;
    private int currentPositionIndex;

    //protected override void Awake()
    //{
    //    base.Awake();
    //    targetPosition = transform.position;
    //}

    // Update is called once per frame
    private void Update()
    {
        if (!isUnitActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];

        Vector3 moveDirection = (targetPosition - transform.position).normalized; 
        float stopDistance = .1f;
        float moveSpeed = 4f;
        float rotateSpeed = 10f;

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count) //reached the end of the position list
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete(onActionComplete);
            }
        }
    }


    public override void TakeAction(GridPosition gridPosition,Action onActionComplete )
    {
        List<GridPosition> pathGridPositionList =  Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>() ;

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }


    public override List<GridPosition> GetValidGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                //int testDistance = Mathf.CeilToInt(Mathf.Sqrt(Mathf.Abs(x) * Mathf.Abs(x) + Mathf.Abs(z) * Mathf.Abs(z)));

                //if (testDistance > maxMoveDistance) continue;//target tile out of move range

                if (!LevelGrid.Instance.IsWithinMapGridSystemRange(testGridPosition)) continue; //indexoutofrangeexception

                if (unitGridPosition == testGridPosition) continue; //unit already at this grid

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue; // multiple units on same grid is not allowed

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) continue;//not a walkable grid

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) continue; //no valid path

                int pathfindingDistanceMultiplier = 10;
                if(Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier) continue; // out of move power

                validGridPositionList.Add(testGridPosition);
                //Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }


    public override string GetActionName()
    {
        return "Move";
    }

    public override int GetActionPointCost()
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {

        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition*10,
        };
    }



}
