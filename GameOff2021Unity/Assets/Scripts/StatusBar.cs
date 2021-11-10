using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
  private ProgressBar progressBar;
  private TextMeshProUGUI numberLabel;

  private int maxValue;
  private int currentValue;

  private void Start()
  {
    progressBar = GetComponentInChildren<ProgressBar>();

    // Number label should be the first text component.
    numberLabel = GetComponentInChildren<TextMeshProUGUI>();
  }

  private void Update()
  {
    numberLabel.text = currentValue + " / " + maxValue;
  }

  public void SetMaxValue(int value)
  {
    maxValue = value;
    progressBar.SetMaxValue(maxValue);
  }

  public void SetValue(int value)
  {
    currentValue = value;
    progressBar.SetValue(currentValue);
  }
}
