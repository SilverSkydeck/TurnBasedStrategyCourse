using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{

    public static GridSystemVisual Instance { get; private set; }

    [Serializable] public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        LightRed,
        Green,
        LightGreen,
        Yellow
    }


    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are more than one GridSystemVisual" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;


    }


    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[ LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight() ];


        for (int x = 0; x< LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition),Quaternion.identity);

                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }


        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitChangedGridPosition += LevelGrid_OnAnyUnitChangedGridPosition;

        UpdateGridVisual();
    }

    //private void Update()
    //{
    //    UpdateGridVisual();
    //}

    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)//for shoot action range display
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x= -range; x <= range; x++)
        {
            for (int z = -range; z<= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x,z);
                if (!LevelGrid.Instance.IsWithinMapGridSystemRange(testGridPosition)) continue;

                int testDistance = Mathf.CeilToInt(Mathf.Sqrt(Mathf.Abs(x) * Mathf.Abs(x) + Mathf.Abs(z) * Mathf.Abs(z)));
                if (testDistance > range) continue;

                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPositionList(gridPositionList, GridVisualType.Yellow);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList , GridVisualType gridVisualType)
    {
        foreach(GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));    
        }


    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Green;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetShootActionRange(), GridVisualType.Yellow);
                break;

        }


        ShowGridPositionList(selectedAction.GetValidGridPositionList(), gridVisualType);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitChangedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
                return gridVisualTypeMaterial.material;
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }



}



