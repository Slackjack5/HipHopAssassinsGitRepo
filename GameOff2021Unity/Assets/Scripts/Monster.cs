using UnityEngine;

public class Monster : Combatant
{
  [SerializeField] private int patternId;

  public int PatternId => patternId;
}
