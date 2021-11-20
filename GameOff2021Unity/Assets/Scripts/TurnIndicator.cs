using System;
using DG.Tweening;
using UnityEngine;

public class TurnIndicator : MonoBehaviour
{
  [SerializeField] private Transform currentTurnIndicator;
  [SerializeField] private float travelTime;
  [SerializeField] private float rotateUnitsPerFrame;

  private RectTransform rectTransform;
  private Vector2 initialPosition;
  private bool isInitialPositionSet;
  private bool isShowing;

  private void Start()
  {
    rectTransform = GetComponent<RectTransform>();
    initialPosition = rectTransform.anchoredPosition;
    isInitialPositionSet = true;
  }

  private void Update()
  {
    if (isShowing)
    {
      transform.Rotate(new Vector3(0, 0, rotateUnitsPerFrame));
    }
  }

  public void Show()
  {
    transform.DOMove(currentTurnIndicator.position, travelTime);
    isShowing = true;
  }

  public void Hide()
  {
    if (!isInitialPositionSet)
    {
      Debug.LogWarning("Tried to reset position of TurnIndicator, but initial position is not set!");
      return;
    }

    rectTransform.DOAnchorPos(initialPosition, travelTime);
    isShowing = false;
  }
}
