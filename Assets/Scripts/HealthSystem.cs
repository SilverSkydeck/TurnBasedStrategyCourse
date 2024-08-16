using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnUnitHealthReachZero;
    public event EventHandler OnUnitHealthChanged;

    [SerializeField] private float health = 15f;
    [SerializeField] private float maxHealth = 20f;

    public void TakeDamage( float damageAmount)
    {
        health -= damageAmount;
        SetHealthToZeroIfBelow();
        //Debug.Log(Mathf.CeilToInt(health));//We leave it be for now

        OnUnitHealthChanged?.Invoke(this, EventArgs.Empty);

        if (health == 0f)
        {
            Die();
        }

    }

    public void SetHealthToZeroIfBelow()
    {
        if (health < 0f) health = 0f; 
    }

    public void Die()
    {
        OnUnitHealthReachZero?.Invoke(this, EventArgs.Empty);
    }

    public float GetNormalizedHealth()
    {
        return health / maxHealth;
    }

    public float GetCurrentHealth()
    {
        return (int)health;
    }

    public float GetMaxHealth()
    {
        return (int)maxHealth;
    }


}
