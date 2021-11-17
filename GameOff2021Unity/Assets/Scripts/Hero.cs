using System;
using DG.Tweening;
using UnityEngine;

public class Hero : Combatant
{
  [SerializeField] private int heroId;
  [SerializeField] private float spotlightDistance;

  private Animator animator;

  public int HeroId => heroId;

  protected override void Awake()
  {
    base.Awake();

    animator = GetComponent<Animator>();
  }

  protected override void ChangeState(State state)
  {
    base.ChangeState(state);

    animator.SetBool("isAttacking", currentState == State.Attacking);
  }

  public void Spotlight()
  {
    transform.DOMoveX(transform.position.x + spotlightDistance, travelTime);
  }
}
