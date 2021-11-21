using UnityEngine;

[System.Serializable]
public class Macro : Command
{
  public int id;
  public int power;

  private bool isInitialized;
  private float effectMultiplier;
  private bool isLastHit;
  private bool hasMissed;

  public override void Execute(Combatant actor)
  {
    if (!isInitialized)
    {
      Debug.LogError("Failed to execute macro for " + actor.Name + ". Macro fields are not initialized!");
      return;
    }

    switch (id)
    {
      case 1:
        // Repair
        Target.IncreaseHealth(Mathf.FloorToInt(power * effectMultiplier));
        break;
      case 2:
        // Mass repair
        foreach (Hero hero in CombatManager.Heroes)
        {
          hero.IncreaseHealth(Mathf.FloorToInt(power * effectMultiplier));
        }

        break;
      case 3:
        // Reboot
        if (isLastHit && !hasMissed)
        {
          Target.Resurrect();
        }

        break;
    }
  }

  public void Execute(Combatant actor, float effectMultiplier, bool isLastHit)
  {
    this.effectMultiplier = effectMultiplier;
    this.isLastHit = isLastHit;
    if (effectMultiplier == 0)
    {
      hasMissed = true;
    }

    isInitialized = true;

    Execute(actor);
  }
}
