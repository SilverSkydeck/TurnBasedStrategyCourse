using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public static event EventHandler OnAnyActionPointChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDied;


    private GridPosition gridPosition;
    private HealthSystem healthSystem;

    private BaseAction[] baseActionArray;

    private int maxActionPoint = 3;

    [SerializeField] private bool isEnemy;
    [SerializeField] private int currentActionPoint;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        baseActionArray = GetComponents<BaseAction>();

    }


    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnEnded += TurnSystem_OnTurnEnded;

        ResetActionPoints();//Don't touch this just yet since we start on turn 0

        healthSystem.OnUnitHealthReachZero += healthSystem_OnUnitHealthReachZero;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        

        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;


            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
          
        }
       

    }

    public T GetAction<T>() where T :BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (HaveEnoughActionPointsToTakeAction(baseAction))
        {
            DeductActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        else return false;
    }


    public bool HaveEnoughActionPointsToTakeAction(BaseAction baseAction)
    {
        return currentActionPoint >= baseAction.GetActionPointCost();
    }

    private void DeductActionPoints(int deductAmount)
    {
        currentActionPoint -= deductAmount;

        OnAnyActionPointChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetCurrentActionPoint()
    {
        return currentActionPoint;
    }

    public int GetMaxActionPoint()
    {
        return maxActionPoint;
    }

    public bool GetIsEnemy()
    {
        return isEnemy;
    }

    public void Damage(float damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);
        //Debug.Log(transform + "damaged!");
    }


    private void TurnSystem_OnTurnEnded(object sender, EventArgs e)
    {
       
        if (GetIsEnemy() && !TurnSystem.Instance.GetIsPlayerTurn()||!GetIsEnemy() && TurnSystem.Instance.GetIsPlayerTurn())  ResetActionPoints();
    }

    private void healthSystem_OnUnitHealthReachZero(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        GridSystemVisual.Instance.HideAllGridPosition(); 
        //no grid visual should be displayed on dead unit. Visual still showing up after another visual update so further fix needed 

        Destroy(gameObject);
        OnAnyUnitDied?.Invoke(this, EventArgs.Empty);
    }


    private void ResetActionPoints()
    {
        currentActionPoint = maxActionPoint;

        OnAnyActionPointChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetNormalizedHealth();
    }

}
