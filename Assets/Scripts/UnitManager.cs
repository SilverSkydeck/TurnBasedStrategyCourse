using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance { get; private set; }

    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are more than one UnitManager" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;


        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();

    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDied += Unit_OnAnyUnitDied;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        //Debug.Log(unit + "spawned");
        unitList.Add(unit);
        if (unit.GetIsEnemy()) enemyUnitList.Add(unit);
        else friendlyUnitList.Add(unit);
    }

    private void Unit_OnAnyUnitDied(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        //Debug.Log(unit + "died");
        unitList.Remove(unit);
        if (unit.GetIsEnemy()) enemyUnitList.Remove(unit);
        else friendlyUnitList.Remove(unit);
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public List<Unit> GetFriendlyUnitList()
    {
        return friendlyUnitList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return enemyUnitList;
    }

}
