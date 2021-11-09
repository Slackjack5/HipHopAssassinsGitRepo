using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{

  public static float totalTime { get; private set; }
  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
    if(GlobalVariables.gameStarted)
    {
    CountTime();
    }
  }

  public void CountTime()
  {
    totalTime += Time.deltaTime;
  }
}
