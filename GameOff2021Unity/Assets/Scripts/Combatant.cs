using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

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

  private Combatant target;

  public int Attack => attack;
  public int CurrentHealth { get; private set; }
  public int CurrentStamina { get; private set; }
  public bool IsDead => CurrentHealth <= 0;

  public int MaxHealth => maxHealth;

  public int MaxStamina => maxStamina;

  public string Name => combatantName;

  public int Speed => speed;

  protected virtual void Start()
  {
    CurrentHealth = maxHealth;
    CurrentStamina = maxStamina;
  }

  public void DecreaseHealth(int value)
  {
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
    CurrentStamina -= value;
    if (CurrentStamina <= 0)
    {
      CurrentStamina = 0;
    }
  }

  public void IncreaseHealth(int value)
  {
    CurrentHealth += value;
    if (CurrentHealth >= maxHealth)
    {
      CurrentHealth = maxHealth;
    }
  }

  public void IncreaseStamina(int value)
  {
    CurrentStamina += value;
    if (CurrentStamina >= maxStamina)
    {
      CurrentStamina = maxStamina;
    }
  }

  public void SetTarget(Combatant combatant)
  {
    target = combatant;
  }

  public void DamageTarget(float damageMultiplier)
  {
    if (target == null)
    {
      Debug.LogError(combatantName + " just tried to damage target, but target is null!");
    }

    MoveToTarget();
    target.DecreaseHealth(Mathf.RoundToInt(Attack * damageMultiplier));
  }

  private void Die()
  {
    GetComponent<SpriteRenderer>().enabled = false;
    dead.Invoke();
  }

  private void SpawnDamageNumber(int value)
  {
    GameObject obj = Instantiate(damageNumberPrefab, damageSpawnTransform);
    obj.GetComponent<DamageNumber>().value = value;
  }

  private void MoveToTarget()
  {
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
