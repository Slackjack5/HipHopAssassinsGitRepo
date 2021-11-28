using DG.Tweening;
using UnityEngine;

public class Hero : Combatant
{
  [SerializeField] private int heroId;
  [SerializeField] private float spotlightDistance;
  [SerializeField] private int attackPatternId;
  [SerializeField] private int[] macroIds;

  private Command submittedCommand;

  public int AttackPatternId => attackPatternId;
  public int HeroId => heroId;
  public int[] MacroIds => macroIds;
  public bool IsSurged { get; private set; }

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
    gameObject.GetComponent<Animator>().SetBool("Hurt", false);
  }

  public void Reset(bool atFirstEncounter = false)
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
}
