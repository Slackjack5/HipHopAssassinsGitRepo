using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
  private Slider slider;

  public bool IsFull
  {
    get { return slider.value >= slider.maxValue; }
  }

  private void Awake()
  {
    slider = GetComponent<Slider>();
  }

  public void Decrease(float value)
  {
    slider.value -= value;
    if (slider.value <= 0)
    {
      slider.value = 0;
    }
  }

  public void Increase(float value)
  {
    slider.value += value;
    if (slider.value >= slider.maxValue)
    {
      slider.value = slider.maxValue;
    }
  }

  public void SetMaxValue(float value)
  {
    slider.maxValue = value;
  }

  public void SetValue(float value)
  {
    slider.value = value;
  }
}
