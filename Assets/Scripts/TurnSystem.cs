using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    private int turnNumber = 0;
    private bool isPlayerTurn = false;


    public event EventHandler OnTurnEnded;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are more than one TurnSystem" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    private void Start()
    {
        NextTurn();
    }

    public void NextTurn()
    {
        if(!isPlayerTurn) turnNumber++;
        isPlayerTurn = !isPlayerTurn;

        OnTurnEnded?.Invoke(this, EventArgs.Empty);
        //Debug.Log("1 time called");

    }
    
    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool GetIsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
