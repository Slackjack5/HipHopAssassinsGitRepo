using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
  [SerializeField] private float timeLimit = 120f;  // In seconds

  private ProgressBar progressBar;

  private float currentTime;

  private void Start()
  {
    progressBar = GetComponentInChildren<ProgressBar>();
    progressBar.SetMaxValue(timeLimit);
    progressBar.SetValue(timeLimit);

    currentTime = timeLimit;
  }

  private void Update()
  {
    currentTime -= Time.deltaTime;
    if (currentTime <= 0)
    {
      currentTime = 0;
    }

    progressBar.SetValue(currentTime);
  }
}
