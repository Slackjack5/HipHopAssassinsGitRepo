using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;

public class CommandPageControl : MonoBehaviour
{
  public KeyControl keyControl;
  public readonly UnityEvent activate = new UnityEvent();

  private bool isReady = false;

  private void Update()
  {
    if (gameObject == EventSystem.current.currentSelectedGameObject)
    {
      if (isReady && keyControl.wasPressedThisFrame)
      {
        activate.Invoke();
      }

      isReady = true;
    }
    else
    {
      isReady = false;
    }
  }
}
