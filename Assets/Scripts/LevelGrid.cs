using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{

    public static LevelGrid Instance { get; private set; }


    public event EventHandler OnAnyUnitChangedGridPosition;

    [SerializeField] private Transform gridDebugObjectPrefab;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;

    private GridSystem<GridObject> gridSystem;




    private void Awake()
    {

        if (Instance != null)
        {
            Debug.LogError("There are more than one UnitActionSystem" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gridSystem = new GridSystem<GridObject>(width, height, cellSize,
                                (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g,gridPosition));
         //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        // Debug.Log(new GridPosition(5,7));
        // Debug.Log(gridSystem.GetGridPosition(MouseWorld.GetPosition()));

    }

    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }

    //private void Update()
    //{
    //    Debug.Log(gridSystem.GetGridPosition(MouseWorld.GetPosition()));
    //}


    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        AddUnitAtGridPosition(toGridPosition, unit);
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        OnAnyUnitChangedGridPosition?.Invoke(this, EventArgs.Empty);
    }


    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return gridSystem.GetGridPosition(worldPosition);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return gridSystem.GetWorldPosition(gridPosition);
    }


    public bool IsWithinMapGridSystemRange(GridPosition gridPosition)
    {
        return gridSystem.IsWithinMapGridSystemRange(gridPosition);
    }

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public int GetWidth()
    {
        return gridSystem.GetWidth();
    }

    public int GetHeight()
    {
        return gridSystem.GetHeight();
    }
}
