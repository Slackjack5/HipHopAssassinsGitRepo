using UnityEngine;

[System.Serializable]
public class Consumable : Command
{
  public int id;
  public int power;
  public int cost;

  private int amountOwned;
  private int lastAmountOwned; // amountOwned right before it is modified.
  private bool _isLastHit;
  private int missCount;
  private bool isInitialized;

  public int AmountOwned => amountOwned;

  public void IncrementAmountOwned()
  {
    lastAmountOwned = amountOwned;
    amountOwned++;
  }

  public void DecrementAmountOwned()
  {
    lastAmountOwned = amountOwned;
    amountOwned--;
  }

  public override void Execute(Combatant actor)
  {
    if (lastAmountOwned == 0)
    {
      Debug.LogError($"Failed to use consumable {name} for {actor.Name}. Amount owned is 0!");
      return;
    }

    if (!isInitialized)
    {
      Debug.LogError($"Failed to use consumable {name} for {actor.Name}. Fields are not initialized!");
      return;
    }

    if (!ShouldExecute()) return;

    switch (id)
    {
      case 1:
        // Bandages
        Target.IncreaseHealth(power);
        break;
      case 2:
        // Attack Candy
        Target.BuffAttack();
        break;
      case 3:
        // Defense Candy
        Target.BuffDefense();
        break;
      case 4:
        // Macro Candy
        Target.BuffMacro();
        break;
      case 5:
        // Reboot Disk
        Target.Resurrect();
        break;
      case 6:
        // Concentrate Candy
        ((Hero) Target).Surge();
        break;
      case 7:
        // Medkit
        Target.IncreaseHealth(power);
        break;
      case 8:
        // Battery
        Target.IncreaseStamina(power);
        break;
      case 9:
        // Energizer
        Target.IncreaseStamina(power);
        break;
    }
  }

  public void Execute(Combatant actor, float effectMultiplier, bool isLastHit)
  {
    _isLastHit = isLastHit;
    if (effectMultiplier == 0)
    {
      missCount++;
    }

    isInitialized = true;

    Execute(actor);
  }

  private bool ShouldExecute()
  {
    // Player can miss up to 2 times before the consumable fails.
    return _isLastHit && missCount <= 2;
  }
}
