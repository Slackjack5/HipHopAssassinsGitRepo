using UnityEngine;

public class Attack : Command
{
  private bool isInitialized;
  private float damageMultiplier;
  private bool isLastHit;

  public override void Execute(Combatant actor)
  {
    if (!isInitialized)
    {
      Debug.LogError("Failed to execute attack for " + actor.Name + ". Attack fields are not initialized!");
      return;
    }

    actor.SetTarget(Target);
    actor.AttackTarget(damageMultiplier, isLastHit);
  }

  public void Execute(Combatant actor, float damageMultiplier, bool isLastHit)
  {
    this.damageMultiplier = damageMultiplier;
    this.isLastHit = isLastHit;
    isInitialized = true;

    Execute(actor);
  }
}
