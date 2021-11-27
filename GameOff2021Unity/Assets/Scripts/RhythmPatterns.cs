using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RhythmPatterns : MonoBehaviour
{
  public static List<float> Pattern(int patternId)
  {
    var beats = new List<float>();
    switch (patternId)
    {
      case 0:
        // DO NOT USE
        break;
      case 1:
        // Analysis attack
        beats.AddRange(new[] {1f, 2, 4});
        break;
      case 2:
        // Unused
        beats.AddRange(new[] {1f, 3, 4});
        break;
      case 3:
        // Unused
        beats.AddRange(new[] {1f, 2, 3});
        break;
      case 4:
        // Initiate attack
        beats.AddRange(new[] {3.5f, 4});
        break;
      case 5:
        // Cockroach attack
        beats.AddRange(new[] {1, 1.5f, 2.5f, 3});
        break;
      case 6:
        // Dragonfly attack
        beats.AddRange(new[] {1, 1.5f, 2, 2.5f, 3.5f, 4});
        break;
      case 7:
        // Vanguard attack
        beats.AddRange(new[] {1, 1.75f, 2.5f, 3, 4});
        break;
      case 8:
        // Repair
        beats.AddRange(new[] {1.5f, 2, 2.5f, 3.5f, 4, 4.5f});
        break;
      case 9:
        // Mass repair
        beats.AddRange(new[] {1.5f, 2, 2.5f, 3.5f, 4, 4.25f, 4.5f});
        break;
      case 10:
        // Reboot
        beats.AddRange(new[] {1, 1.25f, 1.5f, 2, 2.75f, 3, 3.25f, 3.5f, 4});
        break;
      case 11:
        // Debug
        beats.AddRange(new[] {1, 1.75f, 2, 2.5f, 3.5f, 4});
        break;
      case 12:
        // Mass debug
        beats.AddRange(new[] {1, 1.75f, 2, 2.5f, 3, 3.25f, 3.5f, 4});
        break;
      case 13:
        // Macro+
        beats.AddRange(new[] {1, 1.5f, 2, 2.25f, 2.75f, 3.5f});
        break;
      case 14:
        // Attack+
        beats.AddRange(new[] {1, 1.5f, 2, 2.25f, 4, 4.25f, 4.5f});
        break;
      case 15:
        // Defense+
        beats.AddRange(new[] {1, 1.5f, 2, 2.25f, 4, 4.25f});
        break;
      case 16:
        // Surge
        beats.AddRange(new[] {1, 1.5f, 2, 2.25f, 2.75f, 3.25f, 3.75f, 4.25f});
        break;
      case 17:
        // Remove Debuff
        beats.AddRange(new[] {1, 1.25f, 1.5f, 2.5f, 2.75f, 3, 4});
        break;
      case 18:
        // Remove Buff
        beats.AddRange(new[] {1, 1.25f, 1.5f, 2.5f, 3.5f, 3.75f, 4});
        break;
      case 19:
        // Pause
        beats.AddRange(new[] {1, 1.5f, 1.75f, 2.5f, 2.75f, 3.5f, 3.75f, 4.5f});
        break;
      case 20:
        // Bandages
        beats.AddRange(new[] {1.5f, 2, 3.5f, 4});
        break;
      case 21:
        // Attack Candy
        beats.AddRange(new[] {1.5f, 2, 3.25f, 3.5f, 4.5f});
        break;
      case 22:
        // Defense Candy
        beats.AddRange(new[] {1.5f, 2, 2.25f, 3.5f, 4});
        break;
      case 23:
        // Macro Candy
        beats.AddRange(new[] {1.5f, 2, 2.75f, 3, 3.5f, 4});
        break;
      case 24:
        // Reboot Disk
        beats.AddRange(new[] {1.5f, 2, 2.75f, 3.5f, 4.5f});
        break;
      case 25:
        // Concentrate Candy
        beats.AddRange(new[] {1.5f, 2, 2.25f, 2.5f, 2.75f, 3, 3.5f});
        break;
      case 26:
        // Medkit
        beats.AddRange(new[] {1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f});
        break;
      case 27:
        // Battery
        beats.AddRange(new[] {1.5f, 2, 3, 4});
        break;
      case 28:
        // Energizer
        beats.AddRange(new[] {1.5f, 2, 2.75f, 3, 3.75f, 4});
        break;
    }

    return beats.Select(beat => AudioEvents.secondsPerBeat * (beat - 1)).ToList();
  }
}
