using UnityEngine;

[System.Serializable]
public class Macro : Command
{
  public int id;
  public int power;
  public int cost;

  private bool isInitialized;
  private float _effectMultiplier;
  private bool _isLastHit;
  private bool hasMissed;

  public bool HasEnoughStamina { get; set; }

  public override void Execute(Combatant actor)
  {
    if (!isInitialized)
    {
      Debug.LogError($"Failed to execute macro for {actor.Name}. Macro fields are not initialized!");
      return;
    }

    if (!ShouldExecute()) return;

    actor.DecreaseStamina(cost);

    switch (id)
    {
      case 1:
        // Repair
        Target.IncreaseHealth(Mathf.FloorToInt(power * _effectMultiplier));
        break;
      case 2:
        // Mass repair
        foreach (Hero hero in CombatManager.Heroes)
        {
          hero.IncreaseHealth(Mathf.FloorToInt(power * _effectMultiplier));
        }

        break;
      case 3:
        // Reboot
        Target.Resurrect();

        break;
      case 4:
        // Debug
        Target.TakeDamage(actor, _effectMultiplier, true);
        break;
      case 5:
        // Mass debug
        foreach (Monster monster in CombatManager.Monsters)
        {
          monster.TakeDamage(actor, _effectMultiplier, true);
        }

        break;
      case 6:
        // Macro+
        foreach (Hero hero in CombatManager.Heroes)
        {
          hero.BuffMacro();
        }

        IconManager.BuffMacro();
        break;
      case 7:
        // Attack+
        foreach (Hero hero in CombatManager.Heroes)
        {
          hero.BuffAttack();
        }

        IconManager.BuffAttack();
        break;
      case 8:
        // Defense+
        foreach (Hero hero in CombatManager.Heroes)
        {
          hero.BuffDefense();
        }

        IconManager.BuffDefense();
        break;
      case 9:
        // Surge
        ((Hero) Target).Surge();

        break;
      case 10:
        // Remove Debuff
        Target.ResetDebuffMultipliers();

        break;
      case 11:
        // Remove Buff
        Target.ResetBuffMultipliers();

        break;
      case 12:
        // Pause
        Timer.Pause(power);

        break;
    }
  }

  public void Execute(Combatant actor, float effectMultiplier, bool isLastHit)
  {
    _effectMultiplier = effectMultiplier;
    _isLastHit = isLastHit;
    if (effectMultiplier == 0)
    {
      hasMissed = true;
    }

    isInitialized = true;

    Execute(actor);
  }

  private bool ShouldExecute()
  {
    return _isLastHit && !hasMissed;
  }
}
