using UnityEngine;

public class Hero : Combatant
{
  [SerializeField] private int heroId;

  public int HeroId => heroId;
}
