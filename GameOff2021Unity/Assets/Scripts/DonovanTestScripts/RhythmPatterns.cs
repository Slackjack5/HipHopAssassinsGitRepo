using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmPatterns : MonoBehaviour
{
  //Pattersn for Character 1, NO attacks on Beat 3
  public static float[] Pattern(float SPBeat, float SPBar, int PatternNumber)
  {
    //Values Start at -1 to show they aren't set to a time
    float firstTime = -1f;
    float secondTime = -1f;
    float thirdTime = -1f;

    if (PatternNumber == 1)
    {
      firstTime = 0; //First Beat is on Beat 1
      secondTime = SPBeat; //Second Beat is on Beat 2
      thirdTime = SPBeat * 3; //Second Beat is on Beat 4
    }


    //Return, the tiime value for all the hit point
    float[] timespots = new float[] { firstTime, secondTime, thirdTime };
    return timespots;
  }
}
