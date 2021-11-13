using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Combatant
{
  [SerializeField] private int heroId;

  public int HeroId
  {
    get { return heroId; }
  }
}
