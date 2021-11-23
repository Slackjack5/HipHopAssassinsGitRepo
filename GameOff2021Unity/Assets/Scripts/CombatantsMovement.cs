using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CombatantsMovement : MonoBehaviour
{
  [SerializeField] private Transform destination;
  [SerializeField] private float travelTime;

  public readonly UnityEvent onComplete = new UnityEvent();

  private void Start()
  {
    transform.DOMove(destination.position, travelTime).OnComplete(onComplete.Invoke);
  }
}
