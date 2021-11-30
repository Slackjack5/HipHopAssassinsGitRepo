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
  [SerializeField] protected float distanceFromTarget;
  [SerializeField] protected float travelTime;

  public readonly UnityEvent dead = new UnityEvent();

  protected State currentState;
  protected float baseDefenseMultiplier;
  protected float baseMacroDefenseMultiplier;
  protected float tempDamageMultiplier;
  protected float tempDefenseMultiplier;
  protected Animator animator;

  private bool isInitialPositionSet;
  private Vector2 initialPosition;

  protected static readonly int hurt = Animator.StringToHash("Hurt");

  protected enum State
  {
    Idle,
    Attacking,
    Dead
  }

  public string Name => combatantName;
  public int CurrentHealth { get; protected set; }
  public int CurrentStamina { get; protected set; }
  public int MaxHealth => maxHealth;
  public int MaxStamina => maxStamina;
  private int Attack => attack;
  public int Speed => speed;
  public bool IsDead => currentState == State.Dead;
  protected Combatant Target { get; private set; }
  public Button TargetCursor => GetComponentInChildren<Button>();
  private float AttackMultiplier { get; set; }
  private float MacroMultiplier { get; set; }
  protected float DefenseMultiplier { get; set; }
  protected float MacroDefenseMultiplier { get; set; }

  protected virtual void Awake()
  {
    animator = GetComponent<Animator>();

    CurrentHealth = maxHealth;
    CurrentStamina = maxStamina;
    AttackMultiplier = 1;
    MacroMultiplier = 1;

    baseDefenseMultiplier = 1;
    DefenseMultiplier = baseDefenseMultiplier;

    baseMacroDefenseMultiplier = 1;
    MacroDefenseMultiplier = baseMacroDefenseMultiplier;

    tempDamageMultiplier = 1;
    tempDefenseMultiplier = 1;
  }

  protected virtual void Start()
  {
    ChangeState(State.Idle);
  }

  protected void ChangeState(State state)
  {
    currentState = state;
  }

  private void DecreaseHealth(int value)
  {
    if (IsDead) return;

    CurrentHealth -= value;
    DamageNumberSpawner.Spawn(value);

    if (CurrentHealth <= 0)
    {
      Die();
    }
  }

  public void DecreaseStamina(int value)
  {
    if (IsDead) return;

    CurrentStamina -= value;
    if (CurrentStamina <= 0)
    {
      CurrentStamina = 0;
    }
  }

  public void IncreaseHealth(int value)
  {
    if (IsDead) return;

    CurrentHealth += value;
    if (CurrentHealth >= maxHealth)
    {
      CurrentHealth = maxHealth;
    }

    FXManager.SpawnHealFX();
    FXManager.SpawnBuffHeal(this);
  }

  public void IncreaseStamina(int value)
  {
    if (IsDead) return;

    CurrentStamina += value;
    if (CurrentStamina >= maxStamina)
    {
      CurrentStamina = maxStamina;
    }
  }

  public virtual void Resurrect()
  {
    if (!IsDead) return;

    ChangeState(State.Idle);
    CurrentHealth = MaxHealth / 2;
  }

  public void BuffAttack()
  {
    AttackMultiplier *= 2;
    if (AttackMultiplier >= 4)
    {
      AttackMultiplier = 4;
    }

    //FX
    FXManager.SpawnBuffOffense(this);
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

    if (DefenseMultiplier < baseDefenseMultiplier)
    {
      DefenseMultiplier = baseDefenseMultiplier;
    }

    if (MacroDefenseMultiplier < baseMacroDefenseMultiplier)
    {
      MacroDefenseMultiplier = baseMacroDefenseMultiplier;
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

    if (DefenseMultiplier > baseDefenseMultiplier)
    {
      DefenseMultiplier = baseDefenseMultiplier;
    }

    if (MacroDefenseMultiplier > baseMacroDefenseMultiplier)
    {
      MacroDefenseMultiplier = baseMacroDefenseMultiplier;
    }
  }

  protected void ResetTempDefenseMultiplier()
  {
    tempDefenseMultiplier = 1;
  }

  protected void ResetTempDamageMultiplier()
  {
    tempDamageMultiplier = 1;
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
    if (IsDead) return;
    Target = combatant;
  }

  public virtual void AttackTarget(float damageMultiplier, bool isLastHit)
  {
    if (IsDead) return;
    if (Target == null)
    {
      Debug.LogError(combatantName + " just tried to damage Target, but Target is null!");
      return;
    }

    if (currentState == State.Attacking && isLastHit)
    {
      ResetPosition();
      ChangeState(State.Idle);
    }
    else
    {
      MoveToTarget();
    }

    Target.TakeDamage(this, damageMultiplier, false);
  }

  public virtual void TakeDamage(Combatant actor, float damageMultiplier, bool isMacro)
  {
    if (IsDead) return;

    float damage = isMacro
      ? actor.Attack * actor.MacroMultiplier * (1 / MacroDefenseMultiplier) * actor.tempDamageMultiplier *
        (1 / tempDefenseMultiplier) * damageMultiplier
      : actor.Attack * actor.AttackMultiplier * (1 / DefenseMultiplier) * actor.tempDamageMultiplier *
        (1 / tempDefenseMultiplier) * damageMultiplier;

    if (animator != null && damageMultiplier != 0)
    {
      animator.SetBool(hurt, true);
      FXManager.SpawnAttackHit(this, isMacro);
    }

    DecreaseHealth(Mathf.RoundToInt(damage));
  }

  protected virtual void Die()
  {
    CurrentHealth = 0;
    animator.SetBool(hurt, false);
    ChangeState(State.Dead);
    dead.Invoke();
  }

  private void MoveToTarget()
  {
    if (currentState != State.Attacking)
    {
      ChangeState(State.Attacking);
      AkSoundEngine.PostEvent("Play_Approach", gameObject);
    }

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
