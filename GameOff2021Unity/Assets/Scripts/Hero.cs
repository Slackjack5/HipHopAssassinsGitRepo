using DG.Tweening;
using UnityEngine;

public class Hero : Combatant
{
  [SerializeField] private int heroId;
  [SerializeField] private float spotlightDistance;
  [SerializeField] private int attackPatternId;
  [SerializeField] private int[] macroIds;

  private Command submittedCommand;
  private static readonly int hurt = Animator.StringToHash("Hurt");

  public int AttackPatternId => attackPatternId;
  public int HeroId => heroId;
  public int[] MacroIds => macroIds;
  private bool IsSurged { get; set; }

  public void SubmitCommand(Command command)
  {
    submittedCommand = command;
  }

  public Command GetSubmittedCommand()
  {
    if (submittedCommand != null) return submittedCommand;

    Debug.LogError(
      $"Failed to get submitted command of hero {Name}. Submitted command is null! (Did you call SubmitCommand?)");
    return null;
  }

  public void ResetCommand()
  {
    submittedCommand = null;
  }

  public void Defend()
  {
    tempDefenseMultiplier = 2;
  }

  public void Surge()
  {
    IsSurged = true;
  }

  public void Spotlight()
  {
    transform.DOMoveX(transform.position.x + spotlightDistance, travelTime);
  }

  public void EndHurtAnimation()
  {
    gameObject.GetComponent<Animator>().SetBool(hurt, false);
  }

  public override void TakeDamage(Combatant actor, float damageMultiplier, bool isMacro)
  {
    base.TakeDamage(actor, damageMultiplier, isMacro);
    if (damageMultiplier != 0)
    {
      FXManager.SpawnHurtFX();
    }
  }

  public void ResetEverything(bool atFirstEncounter = false)
  {
    ResetCommand();
    ResetPosition();
    ResetBuffMultipliers();
    ResetDebuffMultipliers();
    ResetTempDamageMultiplier();
    ResetTempDefenseMultiplier();
    IsSurged = false;

    if (atFirstEncounter)
    {
      CurrentHealth = maxHealth;
      CurrentStamina = maxStamina;

      ChangeState(State.Idle);
    }
  }

  public bool CanCastMacro(Macro macro)
  {
    return macro.cost <= CurrentStamina;
  }

  public void CheckTemporaries()
  {
    if (IsSurged)
    {
      tempDamageMultiplier = 2;
      IsSurged = false;
    }
    else
    {
      ResetTempDamageMultiplier();
    }

    ResetTempDefenseMultiplier();
  }

  public override void AttackTarget(float damageMultiplier, bool isLastHit)
  {
    if (Target.IsDead)
    {
      SetTarget(CombatManager.FirstLivingMonster);
    }

    base.AttackTarget(damageMultiplier, isLastHit);
  }
}
