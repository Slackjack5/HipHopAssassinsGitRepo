using System.Collections.Generic;
using UnityEngine;

public class RhythmPatterns : MonoBehaviour
{
  public static List<float> Pattern(int patternId)
  {
    var timeSpots = new List<float>();

    switch (patternId)
    {
      case 1:
        // All attacks
        // Beats 1, 2, 4
        timeSpots.Add(0);
        timeSpots.Add(AudioEvents.secondsPerBeat * 1);
        timeSpots.Add(AudioEvents.secondsPerBeat * 3);
        break;
      case 2:
        // All consumables
        // Beats 1, 3, 4
        timeSpots.Add(0);
        timeSpots.Add(AudioEvents.secondsPerBeat * 2);
        timeSpots.Add(AudioEvents.secondsPerBeat * 3);
        break;
      case 3:
        // All macros
        // Beats 1, 2, 3
        timeSpots.Add(0);
        timeSpots.Add(AudioEvents.secondsPerBeat * 1);
        timeSpots.Add(AudioEvents.secondsPerBeat * 2);
        break;
      case 4:
        // All stances
        // Beats 2, 3, 4
        timeSpots.Add(AudioEvents.secondsPerBeat * 1);
        timeSpots.Add(AudioEvents.secondsPerBeat * 2);
        timeSpots.Add(AudioEvents.secondsPerBeat * 3);
        break;
      case 5:
        // All monster attacks
        // Beats 1, 1.5, 2.5, 3
        timeSpots.Add(0);
        timeSpots.Add(AudioEvents.secondsPerBeat * 0.5f);
        timeSpots.Add(AudioEvents.secondsPerBeat * 1.5f);
        timeSpots.Add(AudioEvents.secondsPerBeat * 2);
        break;
    }

    return timeSpots;
  }
}
