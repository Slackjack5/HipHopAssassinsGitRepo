using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapManager : MonoBehaviour
{
  [SerializeField] private GameObject track;
  [SerializeField] private GameObject beatCirclePrefab;
  [SerializeField] private GameObject downbeatBar;
  [SerializeField] private GameObject offbeatBar;
  [SerializeField] private RectTransform spawnerPos;
  [SerializeField] private RectTransform centerPos;
  [SerializeField] private RectTransform endPos;
  [Range(0.00f, 1.2f)] private float leniency = 0.07f;

  public readonly UnityEvent complete = new UnityEvent();
  public readonly UnityEvent<Note, AccuracyGrade> hit = new UnityEvent<Note, AccuracyGrade>();

  private readonly List<Note> notes = new List<Note>();
  private readonly List<GameObject> beatEntities = new List<GameObject>();
  private int nextSpawnIndex;
  private int nextHitIndex;
  private float travelTime;
  private bool isGenerated;
  private bool isReady;
  private float lateBound; // The latest, in seconds, that the player can hit the note before it is considered a miss

  private static readonly int attackType = Animator.StringToHash("AttackType");
  private static readonly int heroHash = Animator.StringToHash("Hero");

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
    if (!GlobalVariables.songStarted) return;

    AudioEvents.SegmentPosition segmentPosition = AudioEvents.GetCurrentSegmentPosition();
    if (!segmentPosition.isValid) return;
    float currentSegmentPosition = segmentPosition.value;

    // AudioEvents.secondsPerBeat is not defined until the first measure starts.
    travelTime = 2 * AudioEvents.secondsPerBeat;

    if (isGenerated)
    {
      if (isReady && Input.GetKeyDown("space"))
      {
        CheckHit(currentSegmentPosition);
      }

      if (nextHitIndex < notes.Count &&
          currentSegmentPosition - notes[nextHitIndex].time > leniency) // Player presses nothing
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

  private void FixedUpdate()
  {
    if (!GlobalVariables.songStarted) return;

    AudioEvents.SegmentPosition segmentPosition = AudioEvents.GetCurrentSegmentPosition();
    if (!segmentPosition.isValid) return;
    float currentSegmentPosition = segmentPosition.value;

    if (isGenerated)
    {
      // Spawn 
      if (nextSpawnIndex < notes.Count &&
          currentSegmentPosition >= notes[nextSpawnIndex].time - travelTime)
      {
        Spawn(nextSpawnIndex);
        nextSpawnIndex++;
      }
    }
  }

  private void Finish()
  {
    // Check that all beat circles are destroyed before closing.
    if (notes.Any(note => note.beatCircle != null)) return;

    foreach (GameObject beatEntity in beatEntities)
    {
      Destroy(beatEntity);
    }

    beatEntities.Clear();

    isGenerated = false;
    HideTrack();
    complete.Invoke();
  }

  public void ForceFinish()
  {
    foreach (Note note in notes)
    {
      Destroy(note.beatCircle);
    }

    foreach (GameObject beatEntity in beatEntities)
    {
      Destroy(beatEntity);
    }

    beatEntities.Clear();

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

    notes.Clear();
    notes.AddRange(generatedNotes);
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

  private void CheckHit(float currentSegmentPosition)
  {
    if (nextHitIndex >= notes.Count || !notes[nextHitIndex].beatCircle) return;

    float error = notes[nextHitIndex].time - currentSegmentPosition;

    if (error >= -lateBound && error <= leniency)
    {
      // Check to see where they hit exactly and give proper rating
      if (error <= leniency / 3)
      {
        hit.Invoke(notes[nextHitIndex], AccuracyGrade.Perfect);
        GameObject.Find("FXManager").GetComponent<FXManager>().SpawnPerfectHit();
      }
      else if (error <= (leniency / 3) * 2)
      {
        hit.Invoke(notes[nextHitIndex], AccuracyGrade.Great);
        GameObject.Find("FXManager").GetComponent<FXManager>().SpawnGreatHit();
      }
      else
      {
        hit.Invoke(notes[nextHitIndex], AccuracyGrade.Good);
        GameObject.Find("FXManager").GetComponent<FXManager>().SpawnGreatHit();
      }

      AkSoundEngine.PostEvent("Play_Cowbell", gameObject);

      notes[nextHitIndex].beatCircle.GetComponent<BeatCircle>().Hit();
      nextHitIndex++;
    }
    else if (error > leniency) // Player hits too early
    {
      hit.Invoke(notes[nextHitIndex], AccuracyGrade.Miss);
      notes[nextHitIndex].beatCircle.GetComponent<BeatCircle>().Hit();
      nextHitIndex++;
    }
    else if (error < -lateBound) // Player hits too late
    {
      hit.Invoke(notes[nextHitIndex], AccuracyGrade.Miss);
      nextHitIndex++;
    }
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

    if (note.combatant is Hero hero)
    {
      // Attack is 1. Macro is 2.
      int value = hero.GetSubmittedCommand() is Macro ? 2 : 1;
      circle.GetComponent<Animator>().SetInteger(attackType, value);

      switch (hero.Name)
      {
        case "Vanguard":
          // Current Hero is Vanguard so make Hero 3.
          circle.GetComponent<Animator>().SetInteger(heroHash, 3);
          break;
        case "Initiate":
          // Current Hero is Initiate so make Hero 1.
          circle.GetComponent<Animator>().SetInteger(heroHash, 1);
          break;
        case "Analysis":
          // Current Hero is Analysis so make Hero 2.
          circle.GetComponent<Animator>().SetInteger(heroHash, 2);
          break;
      }
    }
    else
    {
      circle.GetComponent<Animator>().SetInteger(attackType, 1);

      // Enemy is represented with a value of 4.
      circle.GetComponent<Animator>().SetInteger(heroHash, 4);
    }

    note.beatCircle = circle;
  }

  public void SpawnDownbeat()
  {
    if (!isGenerated) return;

    GameObject bar = Instantiate(downbeatBar, spawnerPos.position, Quaternion.identity, track.transform);
    var beat = bar.GetComponent<BeatEntity>();
    beat.travelTime = travelTime;
    beat.centerPos = centerPos;
    beat.endPos = endPos;
    beat.spawnerPos = spawnerPos;

    beatEntities.Add(bar);
  }

  public void SpawnOffbeat()
  {
    if (!isGenerated) return;

    GameObject bar = Instantiate(offbeatBar, spawnerPos.position, Quaternion.identity, track.transform);
    var beat = bar.GetComponent<BeatEntity>();
    beat.travelTime = travelTime;
    beat.centerPos = centerPos;
    beat.endPos = endPos;
    beat.spawnerPos = spawnerPos;

    beatEntities.Add(bar);
  }
}
