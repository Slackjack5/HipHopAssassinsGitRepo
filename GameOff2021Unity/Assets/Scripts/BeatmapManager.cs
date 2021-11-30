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

  private Dictionary<Combatant, Pattern> _combatantPatterns = new Dictionary<Combatant, Pattern>();
  private List<Note> notes = new List<Note>();
  private readonly Dictionary<Combatant, int> combatantStartIndices = new Dictionary<Combatant, int>();
  private readonly List<GameObject> beatEntities = new List<GameObject>();

  private int nextSpawnIndex;
  private int nextHitIndex;
  private float travelTime;
  private bool isGenerated;
  private bool isInputReady;
  private float lateBound; // The latest, in seconds, that the player can hit the note before it is considered a miss

  private static readonly int attackType = Animator.StringToHash("AttackType");
  private static readonly int heroHash = Animator.StringToHash("Hero");

  public class Note
  {
    internal bool isValid;
    internal float time;
    internal GameObject beatCircle;
    internal Combatant combatant;
    internal bool isLastOfCombatant;
    internal string soundName;
    internal bool isCall;
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
      if (nextHitIndex < notes.Count)
      {
        Note note = notes[nextHitIndex];
        if (!note.isValid)
        {
          nextHitIndex++;
        }
        else if (!note.isCall)
        {
          if (isInputReady && Input.GetKeyDown("space"))
          {
            CheckHit(currentSegmentPosition);
          }

          if (currentSegmentPosition - note.time > leniency) // Player presses nothing
          {
            if (note.beatCircle)
            {
              hit.Invoke(note, AccuracyGrade.Miss);
            }

            nextHitIndex++;
          }
        }
      }

      if (nextHitIndex == notes.Count)
      {
        Finish();
      }

      // Wait one more frame before checking hit since the Space key could have just been pressed for submitting a command.
      isInputReady = true;
    }
    else
    {
      isInputReady = false;
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
      // Spawn note
      if (nextSpawnIndex < notes.Count)
      {
        Note note = notes[nextSpawnIndex];
        if (!note.isValid)
        {
          nextSpawnIndex++;
        }
        else if (currentSegmentPosition >= note.time - travelTime)
        {
          SpawnNote(nextSpawnIndex);
          nextSpawnIndex++;
        }
      }

      // Play call notes
      if (nextHitIndex < notes.Count)
      {
        Note note = notes[nextHitIndex];
        if (!note.isValid)
        {
          nextHitIndex++;
        }
        else if (note.isCall && currentSegmentPosition >= notes[nextHitIndex].time)
        {
          AkSoundEngine.PostEvent(notes[nextHitIndex].soundName, gameObject);
          FXManager.SpawnGreatHit();
          nextHitIndex++;
        }
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

  public void GenerateBeatmap(Dictionary<Combatant, Pattern> combatantPatterns, float startTime)
  {
    _combatantPatterns = combatantPatterns;
    combatantStartIndices.Clear();

    var generatedNotes = new List<Note>();
    var barNumber = 0;
    var noteIndex = 0;
    foreach (var entry in combatantPatterns)
    {
      // Keep track of where in the notes list is the first note of a given Combatant.
      combatantStartIndices.Add(entry.Key, noteIndex);

      Pattern pattern = entry.Value;
      for (var i = 0; i < pattern.beats.Length; i++)
      {
        generatedNotes.Add(new Note
        {
          isValid = true,
          time = ConvertBeatToBarTime(pattern.beats[i].beatNumber) + startTime + AudioEvents.secondsPerBar * barNumber,
          combatant = entry.Key,
          isLastOfCombatant = i == pattern.beats.Length - 1,
          soundName = pattern.beats[i].soundName,
          isCall = pattern.beats[i].isCall,
        });

        noteIndex++;
      }

      barNumber += pattern.barLength;
    }

    notes = generatedNotes;
    nextHitIndex = 0;
    nextSpawnIndex = 0;
    ShowTrack();
    isGenerated = true;
  }

  public void RemoveCombatantNotes(Combatant combatant)
  {
    int start = combatantStartIndices[combatant];
    int i = start;
    while (i < start + _combatantPatterns[combatant].beats.Length)
    {
      Destroy(notes[i].beatCircle);
      notes[i].isValid = false;
      i++;
    }

    Pattern removedPattern = _combatantPatterns[combatant];

    // i should now point to the next Combatant's pattern. If it is right after the current pattern, don't fast-forward
    // the time so that there is time for the next notes to spawn.
    if (i - nextHitIndex <= removedPattern.beats.Length) return;
    while (i < notes.Count)
    {
      notes[i].time -= removedPattern.barLength * AudioEvents.secondsPerBar;
      i++;
    }
  }

  private float ConvertBeatToBarTime(float beatNumber)
  {
    return AudioEvents.secondsPerBeat * (beatNumber - 1);
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
    if (nextHitIndex >= notes.Count) return;

    Note note = notes[nextHitIndex];
    if (note.isCall || !note.beatCircle || !note.isValid) return;

    float error = note.time - currentSegmentPosition;

    if (error >= -lateBound && error <= leniency)
    {
      // Check to see where they hit exactly and give proper rating
      if (error <= leniency / 3)
      {
        hit.Invoke(note, AccuracyGrade.Perfect);
        FXManager.SpawnPerfectHit();
      }
      else if (error <= (leniency / 3) * 2)
      {
        hit.Invoke(note, AccuracyGrade.Great);
        FXManager.SpawnGreatHit();
      }
      else
      {
        hit.Invoke(note, AccuracyGrade.Good);
        FXManager.SpawnGreatHit();
      }

      AkSoundEngine.PostEvent(note.soundName, gameObject);
      
      note.beatCircle.GetComponent<BeatCircle>().Hit();
      nextHitIndex++;
    }
    else if (error > leniency) // Player hits too early
    {
      hit.Invoke(note, AccuracyGrade.Miss);
      note.beatCircle.GetComponent<BeatCircle>().Hit();
      nextHitIndex++;
    }
    else if (error < -lateBound) // Player hits too late
    {
      hit.Invoke(note, AccuracyGrade.Miss);
      nextHitIndex++;
    }
  }

  private void SpawnNote(int spawnIndex)
  {
    Note note = notes[spawnIndex];
    if (note.combatant.IsDead || note.isCall || !note.isValid) return;

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

  public void RhythmTambourine()
  {
    if (!isGenerated) return;
    if (nextHitIndex < notes.Count && notes[nextHitIndex].isCall)
    {
      AkSoundEngine.PostEvent("Play_Tambourine", gameObject);
    }
  }

  public void PlayClap()
  {
    if (!isGenerated) return;
    if (nextHitIndex < notes.Count && !notes[nextHitIndex].isCall)
    {
      AkSoundEngine.PostEvent("Play_Clap", gameObject);
    }
  }
}
