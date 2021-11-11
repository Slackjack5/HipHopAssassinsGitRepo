using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
  public float totalTime;

  public static float TotalTime { get; private set; }

  // Update is called once per frame
  void Update()
  {
    if (GlobalVariables.gameStarted)
    {
      TotalTime += Time.deltaTime;
    }

    totalTime = TotalTime;
  }
}
