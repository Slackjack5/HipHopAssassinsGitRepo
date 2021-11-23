using DG.Tweening;
using UnityEngine;

public class Hero : Combatant
{
  [SerializeField] private int heroId;
  [SerializeField] private float spotlightDistance;

  public int HeroId => heroId;

  protected override void Awake()
  {
    base.Awake();

    if (heroId == 3)
    {
      Die();
    }
  }

  public void Spotlight()
  {
    transform.DOMoveX(transform.position.x + spotlightDistance, travelTime);
  }
}
