using UnityEngine;

public class Monster : Combatant
{
  [SerializeField] private int patternId;

  public int PatternId => patternId;


  public void endHitAnimation()
  {
    gameObject.GetComponent<Animator>().SetBool("Hurt", false);
  }
}
