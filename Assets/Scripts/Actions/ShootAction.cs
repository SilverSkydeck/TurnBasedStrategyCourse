using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{

    private enum State
    {
        Aiming,
        Shooting,
        Cooldown,
    }

    private State currentState;

    [SerializeField] private LayerMask obstaclesLayerMask;

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs: EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private int shootActionRange = 3;
    private float stateTimer;
    private float aimingStateTime = 1f;
    private float shootingStateTime = 0.2f;
    private float cooldownStateTime = 0.5f;

    private Unit targetUnit;
    private bool canShoot;

    private void Update()
    {
        if (!isUnitActive) return;

        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Aiming:
                RotateToFaceTarget();
                break;
            case State.Shooting:
                if(canShoot){
                    Shoot();
                    canShoot = false;
                }
                break;
            case State.Cooldown:
                break;
        }

        if (stateTimer <= 0f) NextState();
    }

    private void NextState()
    {
        switch (currentState)
        {
            case State.Aiming:
                currentState = State.Shooting;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:              
                currentState = State.Cooldown;
                stateTimer = cooldownStateTime;
                break;
            case State.Cooldown:
                ActionComplete(onActionComplete);               
                break;
        }

        //Debug.Log(currentState);
    }

    public override List<GridPosition> GetValidGridPositionList()
    {

        GridPosition unitGridPosition = unit.GetGridPosition(); 

        return GetValidGridPositionList(unitGridPosition);
    }



    public List<GridPosition> GetValidGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();



        for (int x = -shootActionRange; x <= shootActionRange; x++)
        {
            for (int z = -shootActionRange; z <= shootActionRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                int testDistance = Mathf.CeilToInt(Mathf.Sqrt(Mathf.Abs(x) * Mathf.Abs(x) + Mathf.Abs(z) * Mathf.Abs(z)));

                if (testDistance > shootActionRange) continue;//target out of range

                if (!LevelGrid.Instance.IsWithinMapGridSystemRange(testGridPosition)) continue; //indexoutofrangeexception

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue; // no unit on the grid

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.GetIsEnemy() == unit.GetIsEnemy()) continue;// units on same teams

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

                float unitShoulderHeightOffset = 1.7f;
                if (Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeightOffset,
                    shootDirection,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstaclesLayerMask)) continue;//shot blocked by obstacle

                validGridPositionList.Add(testGridPosition);
                //Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {


        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        //Debug.Log("Aiming");
        currentState = State.Aiming;
        stateTimer = aimingStateTime;

        canShoot = true;

        ActionStart(onActionComplete);

    }

    private void Shoot()
    {

        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        } 
        ) ;

        float tempDmg = 7f;
        targetUnit.Damage(tempDmg);
    }
    
    private void RotateToFaceTarget()
    {
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        float rotateSpeed = 8f;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
    }


    public override string GetActionName()
    {
        return "Shoot";
    }
    public override int GetActionPointCost()
    {
        return 1;
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetShootActionRange()
    {
        return shootActionRange;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        float normalizedHealth = targetUnit.GetHealthNormalized();
        
        
        
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = Mathf.RoundToInt(100+ 100f*(1- normalizedHealth)),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidGridPositionList(gridPosition).Count;
    }


}
