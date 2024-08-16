using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitWorldUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;


    private void Start()
    {
        Unit.OnAnyActionPointChanged += Unit_OnActionPointChanged;
        healthSystem.OnUnitHealthChanged += HealthSystem_OnUnitHealthChanged;

        UpdateActionPointText();
        UpdateHealthBar();
        UpdateHealthText();
    }

    private void UpdateActionPointText()
    {
        actionPointText.text = unit.GetCurrentActionPoint().ToString();
    }

    private void Unit_OnActionPointChanged(object sender, EventArgs e)
    {
        UpdateActionPointText();
        
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetNormalizedHealth();

    }

    private void UpdateHealthText()
    {
        healthText.text = healthSystem.GetCurrentHealth() + "/" + healthSystem.GetMaxHealth();
    }


    private void HealthSystem_OnUnitHealthChanged(object sender, EventArgs e)
    {
        UpdateHealthBar();
        UpdateHealthText();
    }


}
