using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CombatantsMovement : MonoBehaviour
{
  [SerializeField] private Transform destination;
  [SerializeField] private float travelTime;
  [SerializeField] private UnityEvent complete;

  private void Start()
  {
    transform.DOMove(destination.position, travelTime).OnComplete(complete.Invoke);
  }
}
