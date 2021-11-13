using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmPatterns : MonoBehaviour
{
  public static float[] Pattern(int patternId)
  {
    //Values Start at -1 to show they aren't set to a time
    float firstTime = -1f;
    float secondTime = -1f;
    float thirdTime = -1f;

    if (patternId == 1)
    {
      firstTime = 0; //First Beat is on Beat 1
      secondTime = AudioEvents.secondsPerBeat * 1; //Second Beat is on Beat 2
      thirdTime = AudioEvents.secondsPerBeat * 3; //Second Beat is on Beat 4
    }
    if (patternId == 2)
    {
      firstTime = 0; //First Beat is on Beat 1
      secondTime = AudioEvents.secondsPerBeat * 2; //Second Beat is on Beat 3
      thirdTime = AudioEvents.secondsPerBeat * 3; //Second Beat is on Beat 4
    }
    if (patternId == 3)
    {
      firstTime = 0; //First Beat is on Beat 1
      secondTime = AudioEvents.secondsPerBeat * 1; //Second Beat is on Beat 2
      thirdTime = AudioEvents.secondsPerBeat * 2; //Second Beat is on Beat 3
    }
    if (patternId == 4)
    {
      firstTime = AudioEvents.secondsPerBeat * 1;
      secondTime = AudioEvents.secondsPerBeat * 2;
      thirdTime = AudioEvents.secondsPerBeat * 3;
    }

    // Return the time value for all the hit point
    float[] timespots = new float[] { firstTime, secondTime, thirdTime };
    return timespots;
  }
}
