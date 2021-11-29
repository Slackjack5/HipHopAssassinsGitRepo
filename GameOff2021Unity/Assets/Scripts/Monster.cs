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
    gameObject.GetComponent<Animator>().SetBool("Hurt", false);
  }
}
