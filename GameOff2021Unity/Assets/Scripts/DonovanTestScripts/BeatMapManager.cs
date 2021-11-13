using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapManager : MonoBehaviour
{
  [SerializeField] private GameObject track;
  [SerializeField] private GameObject beatCircle;
  [SerializeField] private Transform spawnerPos;
  [SerializeField] private Transform centerPos;
  [SerializeField] private Transform endPos;
  [Range(0.00f, 1.2f)] private float leniency = 0.07f;

  public readonly UnityEvent complete = new UnityEvent();
  public readonly UnityEvent<Combatant, AccuracyGrade> hit = new UnityEvent<Combatant, AccuracyGrade>();

  private List<Note> notes = new List<Note>();
  private int nextSpawnIndex = 0;
  private int nextHitIndex = 0;
  private float travelTime;
  private bool isGenerated = false;
  private bool isReady = false;
  private float lateBound;  // The latest, in seconds, that the player can hit the note before it is considered a miss

  private class Note
  {
    internal float time;
    internal GameObject beatCircle;
    internal Combatant combatant;
  }

  public enum AccuracyGrade
  {
    PERFECT, GREAT, GOOD, MISS
  }

  private void Start()
  {
    track.SetActive(false);

    lateBound = leniency / 3;
  }

  private void Update()
  {
    if (GlobalVariables.fightStarted) 
    { 
      // AudioEvents.secondsPerBeat is not defined until the first measure starts.
      travelTime = 2 * AudioEvents.secondsPerBeat;

      if (isGenerated)
      {
        // Spawn 
        if (nextSpawnIndex < notes.Count && AudioEvents.CurrentSegmentPosition >= notes[nextSpawnIndex].time - travelTime)
        {
          Spawn(nextSpawnIndex);
          nextSpawnIndex++;
        }

        if (isReady && Input.GetKeyDown("space"))
        {
          CheckHit();
        }

        if (nextHitIndex < notes.Count && AudioEvents.CurrentSegmentPosition - notes[nextHitIndex].time > leniency) // Player presses nothing
        {
          hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.MISS);
          nextHitIndex++;
        }

        if (nextHitIndex == notes.Count)
        {
          Finish();
        }

        // Wait one more frame before checking hit since the Space key could have just been pressed for submitting a command.
        isReady = true;
      }
      else
      {
        isReady = false;
      }
    }
  }

  public void GenerateBeatmap(Dictionary<Combatant, List<float>> combatantPatterns, float startTime)
  {
    int entryNumber = 0;
    foreach (KeyValuePair<Combatant, List<float>> entry in combatantPatterns)
    {
      List<float> pattern = entry.Value;
      for (int i = 0; i < pattern.Count; i++)
      {
        notes.Add(new Note()
        {
          time = pattern[i] + startTime + AudioEvents.secondsPerBar * entryNumber,
          combatant = entry.Key
        });
      }

      entryNumber++;
    }

    isGenerated = true;
  }

  public void HideTrack()
  {
    track.SetActive(false);
  }

  public void ShowTrack()
  {
    track.SetActive(true);
  }

  private void CheckHit()
  {
    if (nextHitIndex < notes.Count)
    {
      float error = notes[nextHitIndex].time - AudioEvents.CurrentSegmentPosition;

      if (error >= -lateBound && error <= leniency)
      {
        // Check to see where they hit exactly and give proper rating
        if (error <= leniency / 3)
        {
          hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.PERFECT);
        }
        else if (error <= (leniency / 3) * 2)
        {
          hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.GREAT);
        }
        else 
        {
          hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.GOOD);
        }

        AkSoundEngine.PostEvent("Play_Cowbell", gameObject);

        Destroy(notes[nextHitIndex].beatCircle);
        nextHitIndex++;
      }
      else if (error > leniency) // Player hits too early
      {
        hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.MISS);
        Destroy(notes[nextHitIndex].beatCircle);
        nextHitIndex++;
      }
      else if (error < -lateBound) // Player hits too late
      {
        hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.MISS);
        nextHitIndex++;
      }
    }
  }

  private void Finish()
  {
    // Check that the last beat circle is destroyed before closing.
    if (notes[notes.Count - 1].beatCircle == null)
    {
      isGenerated = false;
      complete.Invoke();
    }
  }

  private void Spawn(int spawnIndex)
  {
    GameObject Circle = Instantiate(beatCircle, spawnerPos.position, Quaternion.identity);
    Circle.GetComponent<BeatCircle>().travelTime = travelTime;
    Circle.GetComponent<BeatCircle>().centerPos = centerPos;
    Circle.GetComponent<BeatCircle>().endPos = endPos;
    Circle.GetComponent<BeatCircle>().spawnerPos = spawnerPos;
    notes[spawnIndex].beatCircle = Circle;
  }
}
