using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapManager : MonoBehaviour
{
  [SerializeField] private GameObject track;
  [SerializeField] private GameObject beatCirclePrefab;
  [SerializeField] private Transform spawnerPos;
  [SerializeField] private Transform centerPos;
  [SerializeField] private Transform endPos;
  [Range(0.00f, 1.2f)] private float leniency = 0.07f;

  public readonly UnityEvent complete = new UnityEvent();
  public readonly UnityEvent<Note, AccuracyGrade> hit = new UnityEvent<Note, AccuracyGrade>();

  private List<Note> notes = new List<Note>();
  private int nextSpawnIndex;
  private int nextHitIndex;
  private float travelTime;
  private bool isGenerated;
  private bool isReady;
  private float lateBound; // The latest, in seconds, that the player can hit the note before it is considered a miss

  public class Note
  {
    internal float time;
    internal GameObject beatCircle;
    internal Combatant combatant;
    internal bool isLastOfCombatant;
  }

  public enum AccuracyGrade
  {
    Perfect,
    Great,
    Good,
    Miss
  }

  private void Start()
  {
    track.SetActive(false);

    lateBound = leniency * 2 / 3;
  }

  private void Update()
  {
    if (!GlobalVariables.fightStarted) return;

    // AudioEvents.secondsPerBeat is not defined until the first measure starts.
    travelTime = 2 * AudioEvents.secondsPerBeat;

    if (isGenerated)
    {
      // Spawn 
      if (nextSpawnIndex < notes.Count &&
          AudioEvents.CurrentSegmentPosition >= notes[nextSpawnIndex].time - travelTime)
      {
        Spawn(nextSpawnIndex);
        nextSpawnIndex++;
      }

      if (isReady && Input.GetKeyDown("space"))
      {
        CheckHit();
      }

      if (nextHitIndex < notes.Count &&
          AudioEvents.CurrentSegmentPosition - notes[nextHitIndex].time > leniency) // Player presses nothing
      {
        if (notes[nextHitIndex].beatCircle)
        {
          hit.Invoke(notes[nextHitIndex], AccuracyGrade.Miss);
        }

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

  public void ForceFinish()
  {
    isGenerated = false;
    HideTrack();
  }

  public void GenerateBeatmap(Dictionary<Combatant, List<float>> combatantPatterns, float startTime)
  {
    var generatedNotes = new List<Note>();
    var entryNumber = 0;
    foreach (KeyValuePair<Combatant, List<float>> entry in combatantPatterns)
    {
      List<float> pattern = entry.Value;
      for (var i = 0; i < pattern.Count; i++)
      {
        generatedNotes.Add(new Note
        {
          time = pattern[i] + startTime + AudioEvents.secondsPerBar * entryNumber,
          combatant = entry.Key,
          isLastOfCombatant = i == pattern.Count - 1
        });
      }

      entryNumber++;
    }

    notes = generatedNotes;
    nextHitIndex = 0;
    nextSpawnIndex = 0;
    ShowTrack();
    isGenerated = true;
  }

  public void RemoveCombatantNotes(Combatant combatant)
  {
    foreach (Note note in notes.Where(note => note.combatant == combatant))
    {
      Destroy(note.beatCircle);
    }
  }

  private void HideTrack()
  {
    track.SetActive(false);
  }

  private void ShowTrack()
  {
    track.SetActive(true);
  }

  private void CheckHit()
  {
    if (nextHitIndex >= notes.Count || !notes[nextHitIndex].beatCircle) return;

    float error = notes[nextHitIndex].time - AudioEvents.CurrentSegmentPosition;

    if (error >= -lateBound && error <= leniency)
    {
      // Check to see where they hit exactly and give proper rating
      if (error <= leniency / 3)
      {
        hit.Invoke(notes[nextHitIndex], AccuracyGrade.Perfect);
      }
      else if (error <= (leniency / 3) * 2)
      {
        hit.Invoke(notes[nextHitIndex], AccuracyGrade.Great);
      }
      else
      {
        hit.Invoke(notes[nextHitIndex], AccuracyGrade.Good);
      }

      AkSoundEngine.PostEvent("Play_Cowbell", gameObject);

      Destroy(notes[nextHitIndex].beatCircle);
      nextHitIndex++;
    }
    else if (error > leniency) // Player hits too early
    {
      hit.Invoke(notes[nextHitIndex], AccuracyGrade.Miss);
      Destroy(notes[nextHitIndex].beatCircle);
      nextHitIndex++;
    }
    else if (error < -lateBound) // Player hits too late
    {
      hit.Invoke(notes[nextHitIndex], AccuracyGrade.Miss);
      nextHitIndex++;
    }
  }

  private void Finish()
  {
    // Check that the last beat circle is destroyed before closing.
    if (notes[notes.Count - 1].beatCircle != null) return;

    isGenerated = false;
    HideTrack();
    complete.Invoke();
  }

  private void Spawn(int spawnIndex)
  {
    Note note = notes[spawnIndex];
    if (note.combatant.IsDead) return;

    GameObject circle = Instantiate(beatCirclePrefab, spawnerPos.position, Quaternion.identity, track.transform);
    var beatCircle = circle.GetComponent<BeatCircle>();
    beatCircle.travelTime = travelTime;
    beatCircle.centerPos = centerPos;
    beatCircle.endPos = endPos;
    beatCircle.spawnerPos = spawnerPos;

    circle.GetComponent<SpriteRenderer>().color = note.combatant.gameObject.GetComponent<SpriteRenderer>().color;

    note.beatCircle = circle;
  }
}
