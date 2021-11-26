using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Combatant : MonoBehaviour
{
  [SerializeField] protected string combatantName;
  [SerializeField] protected int maxHealth;
  [SerializeField] protected int maxStamina;
  [SerializeField] protected int attack;
  [SerializeField] protected int speed;
  [SerializeField] protected GameObject damageNumberPrefab;
  [SerializeField] protected RectTransform damageSpawnTransform;
  [SerializeField] protected float distanceFromTarget;
  [SerializeField] protected float travelTime;

  public readonly UnityEvent dead = new UnityEvent();

  protected State currentState;
  private bool isInitialPositionSet;
  private Vector2 initialPosition;

  protected enum State
  {
    Idle,
    Attacking,
    Dead
  }

  public string Name => combatantName;
  public int CurrentHealth { get; private set; }
  public int CurrentStamina { get; private set; }
  public int MaxHealth => maxHealth;
  public int MaxStamina => maxStamina;
  public int Attack => attack;
  public int Speed => speed;
  public bool IsDead => CurrentHealth <= 0;
  private Combatant Target { get; set; }
  public Button TargetCursor => GetComponentInChildren<Button>();
  public float AttackMultiplier { get; private set; }
  public float MacroMultiplier { get; private set; }
  public float DefenseMultiplier { get; private set; }

  protected virtual void Awake()
  {
    CurrentHealth = maxHealth;
    CurrentStamina = maxStamina;
    AttackMultiplier = 1;
    MacroMultiplier = 1;
    DefenseMultiplier = 1;
  }

  protected virtual void Start()
  {
    ChangeState(State.Idle);
  }

  protected void ChangeState(State state)
  {
    currentState = state;
  }

  public void DecreaseHealth(int value)
  {
    if (currentState == State.Dead) return;

    CurrentHealth -= value;
    SpawnDamageNumber(value);

    if (CurrentHealth <= 0)
    {
      Die();
    }
  }

  public void DecreaseStamina(int value)
  {
    if (currentState == State.Dead) return;

    CurrentStamina -= value;
    if (CurrentStamina <= 0)
    {
      CurrentStamina = 0;
    }
  }

  public void IncreaseHealth(int value)
  {
    if (currentState == State.Dead) return;

    CurrentHealth += value;
    if (CurrentHealth >= maxHealth)
    {
      CurrentHealth = maxHealth;
    }
  }

  public void IncreaseStamina(int value)
  {
    if (currentState == State.Dead) return;

    CurrentStamina += value;
    if (CurrentStamina >= maxStamina)
    {
      CurrentStamina = maxStamina;
    }
  }

  public void Resurrect()
  {
    if (!IsDead) return;
    CurrentHealth = MaxHealth / 2;
  }

  public void BuffAttack()
  {
    AttackMultiplier *= 2;
    if (AttackMultiplier >= 4)
    {
      AttackMultiplier = 4;
    }
  }

  public void BuffMacro()
  {
    MacroMultiplier *= 2;
    if (MacroMultiplier >= 4)
    {
      MacroMultiplier = 4;
    }
  }

  public void BuffDefense()
  {
    DefenseMultiplier *= 2;
    if (DefenseMultiplier >= 4)
    {
      DefenseMultiplier = 4;
    }
  }

  public void Surge()
  {
    AttackMultiplier *= 2;
    MacroMultiplier *= 2;
  }

  public void ResetDebuffMultipliers()
  {
    if (AttackMultiplier < 1)
    {
      AttackMultiplier = 1;
    }

    if (MacroMultiplier < 1)
    {
      MacroMultiplier = 1;
    }

    if (DefenseMultiplier < 1)
    {
      DefenseMultiplier = 1;
    }
  }

  public void ResetBuffMultipliers()
  {
    if (AttackMultiplier > 1)
    {
      AttackMultiplier = 1;
    }

    if (MacroMultiplier > 1)
    {
      MacroMultiplier = 1;
    }

    if (DefenseMultiplier > 1)
    {
      DefenseMultiplier = 1;
    }
  }

  public void SetInitialPosition()
  {
    initialPosition = transform.position;
    isInitialPositionSet = true;
  }

  public void ResetPosition()
  {
    if (!isInitialPositionSet)
    {
      Debug.LogWarning(combatantName + " could not reset its position. Initial position was not set first.");
      return;
    }

    transform.DOMove(initialPosition, travelTime);
  }

  public void SetTarget(Combatant combatant)
  {
    if (currentState == State.Dead) return;
    Target = combatant;
  }

  public void DamageTarget(float damageMultiplier, bool isLastHit)
  {
    if (currentState == State.Dead) return;
    if (Target == null)
    {
      Debug.LogError(combatantName + " just tried to damage Target, but Target is null!");
      return;
    }

    if (currentState == State.Idle)
    {
      MoveToTarget();
    }

    if (currentState == State.Attacking && isLastHit)
    {
      ResetPosition();
      ChangeState(State.Idle);
    }

    Target.DecreaseHealth(
      Mathf.RoundToInt(Attack * AttackMultiplier * (1 / Target.DefenseMultiplier) * damageMultiplier));

    //Animations
    //Play Hurt Animation
    if (Target.GetComponent<Animator>() != null && damageMultiplier != 0)
    {
      Target.GetComponent<Animator>().SetBool("Hurt", true);
    }
  }

  protected virtual void Die()
  {
    CurrentHealth = 0;
    ChangeState(State.Dead);
    dead.Invoke();
  }

  private void SpawnDamageNumber(int value)
  {
    GameObject obj = Instantiate(damageNumberPrefab, damageSpawnTransform);
    obj.GetComponent<DamageNumber>().value = value;
  }

  private void MoveToTarget()
  {
    ChangeState(State.Attacking);

    Vector2 targetPosition = Target.transform.position;
    float distance = distanceFromTarget;
    if (Target is Monster)
    {
      // Hero is attacking from the left of the Monster.
      distance *= -1;
    }

    transform.DOMove(new Vector2(targetPosition.x + distance, targetPosition.y), travelTime);
  }
}
