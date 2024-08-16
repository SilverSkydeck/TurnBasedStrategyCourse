using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{

    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList;

    public GridObject( GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
        unitList = new List<Unit>();

    }

    public override string ToString()
    {

        string unitString = " ";

        foreach (Unit unit in unitList)
        {
            unitString += "\n" + unit;
        }
        return gridPosition.ToString() + unitString;
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit()) return unitList[0];
        else return null;
    }

    public bool HasAnyUnit()
    {
        return unitList.Count > 0 ;
    }

}
