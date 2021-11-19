using System;
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
  private Combatant target;

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
  public Button TargetCursor { get; private set; }

  protected virtual void Awake()
  {
    CurrentHealth = maxHealth;
    CurrentStamina = maxStamina;

    TargetCursor = GetComponentInChildren<Button>();
  }

  protected virtual void Start()
  {
    ChangeState(State.Idle);
  }

  protected virtual void ChangeState(State state)
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
      CurrentHealth = 0;
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
    target = combatant;
  }

  public void DamageTarget(float damageMultiplier, bool isLastHit)
  {
    if (currentState == State.Dead) return;
    if (target == null)
    {
      Debug.LogError(combatantName + " just tried to damage target, but target is null!");
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

    target.DecreaseHealth(Mathf.RoundToInt(Attack * damageMultiplier));
  }

  private void Die()
  {
    ChangeState(State.Dead);
    dead.Invoke();
    gameObject.SetActive(false);
  }

  private void SpawnDamageNumber(int value)
  {
    GameObject obj = Instantiate(damageNumberPrefab, damageSpawnTransform);
    obj.GetComponent<DamageNumber>().value = value;
  }

  private void MoveToTarget()
  {
    ChangeState(State.Attacking);

    Vector2 targetPosition = target.transform.position;
    float distance = distanceFromTarget;
    if (target is Monster)
    {
      // Hero is attacking from the left of the Monster.
      distance *= -1;
    }

    transform.DOMove(new Vector2(targetPosition.x + distance, targetPosition.y), travelTime);
  }
}
