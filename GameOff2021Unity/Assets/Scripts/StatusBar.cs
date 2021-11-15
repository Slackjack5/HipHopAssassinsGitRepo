using TMPro;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
  private ProgressBar progressBar;
  private TextMeshProUGUI numberLabel;

  private int maxValue;
  private int currentValue;

  private void Awake()
  {
    progressBar = GetComponentInChildren<ProgressBar>();

    // Number label should be the second text component.
    numberLabel = GetComponentsInChildren<TextMeshProUGUI>()[1];
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
