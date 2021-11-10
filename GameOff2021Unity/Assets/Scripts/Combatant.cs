using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combatant : MonoBehaviour
{
  [SerializeField] public string combatantName;
  [SerializeField] public int maxHealth;
  [SerializeField] public int maxStamina;
  [SerializeField] public int speed;

  public int CurrentHealth { get; protected set; }
  public int CurrentStamina { get; protected set; }

  protected virtual void Start()
  {
    CurrentHealth = maxHealth;
    CurrentStamina = maxStamina;
  }

  public void DecreaseHealth(int value)
  {
    CurrentHealth -= value;
    if (CurrentHealth <= 0)
    {
      CurrentHealth = 0;
    }
  }

  public void DecreaseStamina(int value)
  {
    CurrentStamina -= value;
    if (CurrentStamina <= 0)
    {
      CurrentStamina = 0;
    }
  }

  public void IncreaseHealth(int value)
  {
    CurrentHealth += value;
    if (CurrentHealth >= maxHealth)
    {
      CurrentHealth = maxHealth;
    }
  }

  public void IncreaseStamina(int value)
  {
    CurrentStamina += value;
    if (CurrentStamina >= maxStamina)
    {
      CurrentStamina = maxStamina;
    }
  }
}
