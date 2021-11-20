using DG.Tweening;
using UnityEngine;

public class TurnIndicator : MonoBehaviour
{
  [SerializeField] private Transform currentTurnIndicator;
  [SerializeField] private float travelTime;

  private RectTransform rectTransform;
  private Vector2 initialPosition;
  private bool isInitialPositionSet;

  private void Start()
  {
    rectTransform = GetComponent<RectTransform>();
    initialPosition = rectTransform.anchoredPosition;
    isInitialPositionSet = true;

    print(initialPosition);
  }

  public void Show()
  {
    transform.DOMove(currentTurnIndicator.position, travelTime);
  }

  public void Hide()
  {
    if (!isInitialPositionSet)
    {
      Debug.LogWarning("Tried to reset position of TurnIndicator, but initial position is not set!");
      return;
    }

    rectTransform.DOAnchorPos(initialPosition, travelTime);
  }
}
