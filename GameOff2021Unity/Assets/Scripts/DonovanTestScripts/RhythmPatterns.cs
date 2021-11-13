using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmPatterns : MonoBehaviour
{
  public static List<float> Pattern(int patternId)
  {
    List<float> timeSpots = new List<float>();

    if (patternId == 1)
    {
      timeSpots.Add(0); // Beat 1
      timeSpots.Add(AudioEvents.secondsPerBeat * 1); // Beat 2
      timeSpots.Add(AudioEvents.secondsPerBeat * 3); // Beat 4
    }
    if (patternId == 2)
    {
      timeSpots.Add(0);  // Beat 1
      timeSpots.Add(AudioEvents.secondsPerBeat * 2); // Beat 3
      timeSpots.Add(AudioEvents.secondsPerBeat * 3); // Beat 4
    }
    if (patternId == 3)
    {
      timeSpots.Add(0);  // Beat 1
      timeSpots.Add(AudioEvents.secondsPerBeat * 1); // Beat 2
      timeSpots.Add(AudioEvents.secondsPerBeat * 2); // Beat 3
    }
    if (patternId == 4)
    {
      timeSpots.Add(AudioEvents.secondsPerBeat * 1); // Beat 2
      timeSpots.Add(AudioEvents.secondsPerBeat * 2); // Beat 3
      timeSpots.Add(AudioEvents.secondsPerBeat * 3); // Beat 4
    }
    if (patternId == 5)
    {
      timeSpots.Add(0); // Beat 1
      timeSpots.Add(AudioEvents.secondsPerBeat * 0.5f); // Beat 1.5
      timeSpots.Add(AudioEvents.secondsPerBeat * 1.5f); // Beat 2.5
      timeSpots.Add(AudioEvents.secondsPerBeat * 2); // Beat 3
    }

    return timeSpots;
  }
}
