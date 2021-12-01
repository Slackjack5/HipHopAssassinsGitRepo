using DG.Tweening;
using UnityEngine;

public class Monster : Combatant
{
  [SerializeField] private int patternId;
  [SerializeField] private bool isPhysicalResistant;
  [SerializeField] private bool isMacroResistant;
  [SerializeField] private bool doesAoeAttack;
  [SerializeField] private float twitchDistance;
  [SerializeField] private float twitchDuration;

  private bool twitched;

  public int PatternId => patternId;

  protected override void Awake()
  {
    base.Awake();

    if (isPhysicalResistant)
    {
      baseDefenseMultiplier = 2;
      DefenseMultiplier = 2;
    }

    if (isMacroResistant)
    {
      baseMacroDefenseMultiplier = 2;
      MacroDefenseMultiplier = 2;
    }
  }

  protected override void Die()
  {
    base.Die();

    GetComponent<SpriteRenderer>().enabled = false;
    AkSoundEngine.PostEvent("Play_EnemyDeath", gameObject);
    FXManager.SpawnAttackHit(this, true);
  }

  public void endHitAnimation()
  {
    if (animator != null)
    {
      animator.SetBool(hurt, false);
    }
  }

  public override void AttackTarget(float damageMultiplier, bool isLastHit)
  {
    if (doesAoeAttack && !IsDead)
    {
      foreach (Hero hero in CombatManager.Heroes)
      {
        hero.TakeDamage(this, damageMultiplier, false);
      }
    }
    else
    {
      base.AttackTarget(damageMultiplier, isLastHit);
    }
  }

  public void Twitch()
  {
    var position = transform.position;
    if (twitched)
    {
      transform.DOMoveY(position.y - twitchDistance, twitchDuration);
      twitched = false;
    }
    else
    {
      transform.DOMoveY(position.y + twitchDistance, twitchDuration);
      twitched = true;
    }
  }
}
