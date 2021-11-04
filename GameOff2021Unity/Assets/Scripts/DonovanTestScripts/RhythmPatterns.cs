using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmPatterns : MonoBehaviour
{
  //Pattersn for Character 1, NO attacks on Beat 3
  public static float[] Pattern1(float SPBeat, float SPBar)
  {
    float firstTime = 1.2f;
    float secondTime = 2.3f;
    float thirdTime = 3.5f;

    float[] timespots = new float[] { firstTime, secondTime, thirdTime };
    return timespots;
  }
}
