using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ActionButtonUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI actionPointCostText;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedButtonUI;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        actionNameText.text = baseAction.GetActionName().ToUpper();
        actionPointCostText.text = baseAction.GetActionPointCost().ToString();

        button.onClick.AddListener(() => 
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        }  
        );

    }


    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedButtonUI.SetActive(baseAction == selectedBaseAction);
    }



}

