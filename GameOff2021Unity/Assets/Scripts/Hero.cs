using DG.Tweening;
using UnityEngine;

public class Hero : Combatant
{
  [SerializeField] private int heroId;
  [SerializeField] private float spotlightDistance;

  private Animator animator;
  private static readonly int isAttacking = Animator.StringToHash("isAttacking");

  public int HeroId => heroId;

  protected override void Awake()
  {
    base.Awake();

    animator = GetComponent<Animator>();
  }

  protected override void Start()
  {
    base.Start();

    CombatManager.onStateChange.AddListener(OnCombatStateChange);
  }

  private void OnCombatStateChange(CombatManager.CombatState state)
  {
    switch (state)
    {
      case CombatManager.CombatState.Unspecified:
      case CombatManager.CombatState.PreStart:
      case CombatManager.CombatState.Start:
        break;
      case CombatManager.CombatState.HeroOne:
        if (heroId == 1)
        {
          Spotlight();
        }
        else
        {
          ResetPosition();
        }

        break;
      case CombatManager.CombatState.HeroTwo:
        if (heroId == 2)
        {
          Spotlight();
        }
        else
        {
          ResetPosition();
        }

        break;
      case CombatManager.CombatState.HeroThree:
        if (heroId == 3)
        {
          Spotlight();
        }
        else
        {
          ResetPosition();
        }

        break;
      default:
        ResetPosition();
        break;
    }
  }

  protected override void ChangeState(State state)
  {
    base.ChangeState(state);

    animator.SetBool(isAttacking, currentState == State.Attacking);
  }

  private void Spotlight()
  {
    transform.DOMoveX(transform.position.x + spotlightDistance, travelTime);
  }
}
