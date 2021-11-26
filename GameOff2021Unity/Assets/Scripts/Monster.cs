using UnityEngine;

public class Monster : Combatant
{
  [SerializeField] private int patternId;

  public int PatternId => patternId;

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
