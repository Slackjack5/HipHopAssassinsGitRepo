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

  private readonly List<Note> notes = new List<Note>();
  private int nextSpawnIndex;
  private int nextHitIndex;
  private float travelTime;
  private bool isGenerated;
  private bool isReady;
  private float lateBound;  // The latest, in seconds, that the player can hit the note before it is considered a miss

  private class Note
  {
    internal float time;
    internal GameObject beatCircle;
    internal Combatant combatant;
  }

  public enum AccuracyGrade
  {
    Perfect, Great, Good, Miss
  }

  private void Start()
  {
    track.SetActive(false);

    lateBound = leniency / 3;
  }

  private void Update()
  {
    if (!GlobalVariables.fightStarted) return;
    
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
        hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.Miss);
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

  public void GenerateBeatmap(Dictionary<Combatant, List<float>> combatantPatterns, float startTime)
  {
    var entryNumber = 0;
    foreach (KeyValuePair<Combatant, List<float>> entry in combatantPatterns)
    {
      List<float> pattern = entry.Value;
      foreach (float t in pattern)
      {
        notes.Add(new Note()
        {
          time = t + startTime + AudioEvents.secondsPerBar * entryNumber,
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
    if (nextHitIndex >= notes.Count) return;
    
    float error = notes[nextHitIndex].time - AudioEvents.CurrentSegmentPosition;

    if (error >= -lateBound && error <= leniency)
    {
      // Check to see where they hit exactly and give proper rating
      if (error <= leniency / 3)
      {
        hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.Perfect);
      }
      else if (error <= (leniency / 3) * 2)
      {
        hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.Great);
      }
      else 
      {
        hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.Good);
      }

      AkSoundEngine.PostEvent("Play_Cowbell", gameObject);

      Destroy(notes[nextHitIndex].beatCircle);
      nextHitIndex++;
    }
    else if (error > leniency) // Player hits too early
    {
      hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.Miss);
      Destroy(notes[nextHitIndex].beatCircle);
      nextHitIndex++;
    }
    else if (error < -lateBound) // Player hits too late
    {
      hit.Invoke(notes[nextHitIndex].combatant, AccuracyGrade.Miss);
      nextHitIndex++;
    }
  }

  private void Finish()
  {
    // Check that the last beat circle is destroyed before closing.
    if (notes[notes.Count - 1].beatCircle != null) return;
    
    isGenerated = false;
    complete.Invoke();
  }

  private void Spawn(int spawnIndex)
  {
    GameObject circle = Instantiate(beatCircle, spawnerPos.position, Quaternion.identity);
    circle.GetComponent<BeatCircle>().travelTime = travelTime;
    circle.GetComponent<BeatCircle>().centerPos = centerPos;
    circle.GetComponent<BeatCircle>().endPos = endPos;
    circle.GetComponent<BeatCircle>().spawnerPos = spawnerPos;
    notes[spawnIndex].beatCircle = circle;
  }
}
