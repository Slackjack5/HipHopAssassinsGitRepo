using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
  [SerializeField] public string heroName;
  [SerializeField] public int maxHealth;
  [SerializeField] public int maxStamina;

  public int CurrentHealth { get; private set; }
  public int CurrentStamina { get; private set; }

  private void Start()
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
