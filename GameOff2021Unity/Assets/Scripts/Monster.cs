using UnityEngine;

public class Monster : Combatant
{
  [SerializeField] private int patternId;
  [SerializeField] private bool isPhysicalResistant;
  [SerializeField] private bool isMacroResistant;

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
      baseMacroDefenseMultiplier = 0.5f;
      MacroDefenseMultiplier = 0.5f;
    }
  }

  protected override void Die()
  {
    base.Die();

    GetComponent<SpriteRenderer>().enabled = false;
  }

  public void endHitAnimation()
  {
    gameObject.GetComponent<Animator>().SetBool("Hurt", false);
  }
}
