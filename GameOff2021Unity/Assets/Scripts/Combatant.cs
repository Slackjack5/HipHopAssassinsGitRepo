using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combatant : MonoBehaviour
{
  [SerializeField] protected string combatantName;
  [SerializeField] protected int maxHealth;
  [SerializeField] protected int maxStamina;
  [SerializeField] protected int attack;
  [SerializeField] protected int speed;

  public int Attack
  {
    get { return attack; }
  }
  public int CurrentHealth { get; protected set; }
  public int CurrentStamina { get; protected set; }
  public bool IsDead
  {
    get { return CurrentHealth <= 0; }
  }
  public int MaxHealth
  { 
    get { return maxHealth; } 
  }
  public int MaxStamina
  { 
    get { return maxStamina; } 
  }
  public string Name
  {
    get { return combatantName; }
  }
  public int Speed 
  { 
    get { return speed; } 
  }

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
