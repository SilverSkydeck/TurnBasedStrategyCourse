using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnActionButtonUIBusy;
    public event EventHandler OnActionStarted;

    [SerializeField] private LayerMask unitsLayerMask;
    [SerializeField] private Unit selectedUnit;
    private bool isUnitBusy;

    private BaseAction selectedAction;

    private void Awake()
    {
        if (Instance != null)
        { 
            Debug.LogError("There are more than one UnitActionSystem" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }



    private void Update()
    {
        if (isUnitBusy) return;
        if (!TurnSystem.Instance.GetIsPlayerTurn()) return;
        if (TryHandleUnitSelection()) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) return;

            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)) return;

            SetUnitBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearUnitBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
            //switch (selectedAction)
            //{
            //    case MoveAction moveAction:

            //        if (moveAction.IsValidActionGridPosition(mouseGridPosition))
            //        {
            //            SetUnitBusy();
            //            moveAction.TakeAction(mouseGridPosition,ClearUnitBusy);
            //        }
            //        else
            //        {
            //            Debug.Log("Invalid grid");
            //        }
            //        break;

            //    case SpinAction spinAction:
            //        SetUnitBusy();
            //        spinAction.TakeAction(ClearUnitBusy);
            //        break;
            //}
        }
        }


    
    private void SetUnitBusy()
    {
        isUnitBusy = true;

        OnActionButtonUIBusy?.Invoke(this, isUnitBusy);
    }

    private void ClearUnitBusy()
    {
        isUnitBusy = false;

        OnActionButtonUIBusy?.Invoke(this, isUnitBusy);
    }


    private bool TryHandleUnitSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitsLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == selectedUnit) return false;

                    if (unit.GetIsEnemy()) 
                    {
                        //Debug.Log("Enemy");
                        return false; 
                    }

                    SetSelectedUnit(unit);
                    return true;
                }
            }; 
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);

    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() 
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }


}
