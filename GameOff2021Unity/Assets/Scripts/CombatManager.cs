using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
  [SerializeField] private BeatmapManager beatmapManager;
  [SerializeField] private Timer timer;
  [SerializeField] private Hero[] heroes;
  [SerializeField] private Monster[] monsters;

  private float executionStartTime;
  private int lastBar;
  private readonly Command[] submittedCommands = new Command[3];

  public enum CombatState
  {
    UNSPECIFIED, START, HERO_ONE, HERO_TWO, HERO_THREE, DELAY_EXECUTION, PRE_EXECUTION, EXECUTION, WIN, LOSE
  }

  /// <summary>
  /// Combatants sorted in initiative order
  /// </summary>
  public List<Combatant> Combatants { get; private set; }
  public CombatState CurrentState { get; private set; }
  public Hero[] Heroes
  {
    get { return heroes; }
  }

  private void Awake()
  {
    beatmapManager.complete.AddListener(AdvanceState);
    beatmapManager.hit.AddListener(ReadHit);
    timer.expire.AddListener(Lose);

    SortByInitiative();

    CurrentState = CombatState.START;
  }

  private void OnGUI()
  {
    for (int i = 0; i < monsters.Length; i++)
    {
      GUI.Label(new Rect(0, 30 * i, 200, 30), monsters[i].Name + " HP: " + monsters[i].CurrentHealth + " / " + monsters[i].MaxHealth);
    }
  }

  private void Update()
  {
    switch (CurrentState)
    {
      case CombatState.START:
        if (GlobalVariables.fightStarted)
        {
          CurrentState = CombatState.HERO_ONE;
        }
        
        break;
      case CombatState.DELAY_EXECUTION:
        if (lastBar != GlobalVariables.currentBar && (AudioEvents.CurrentBarTime >= Threshold(AudioEvents.secondsPerBar)))
        {
          CurrentState = CombatState.PRE_EXECUTION;
        }
        break;
      case CombatState.PRE_EXECUTION:
        //Generate All Our Patterns
        Dictionary<Combatant, List<float>> combatantPatterns = new Dictionary<Combatant, List<float>>();

        foreach (Combatant combatant in Combatants)
        {
          if (combatant is Hero)
          {
            // HeroId is either 1, 2, or 3.
            Command command = submittedCommands[(combatant as Hero).HeroId - 1];
            combatantPatterns[combatant] = RhythmPatterns.Pattern(command.PatternId);
          }
          else
          {
            // Combatant is a Monster.
            combatantPatterns[combatant] = RhythmPatterns.Pattern(5);
          }
        }

        //Start Recording Time
        executionStartTime = GlobalVariables.currentBar * AudioEvents.secondsPerBar;
        beatmapManager.GenerateBeatmap(combatantPatterns, executionStartTime);

        CurrentState = CombatState.EXECUTION;
        break;
      default:
        break;
    }
  }

  public void AdvanceState()
  {
    switch (CurrentState)
    {
      case CombatState.HERO_ONE:
        CurrentState = CombatState.HERO_TWO;
        break;
      case CombatState.HERO_TWO:
        CurrentState = CombatState.HERO_THREE;
        break;
      case CombatState.HERO_THREE:
        beatmapManager.ShowTrack();

        if (AudioEvents.CurrentBarTime <= Threshold(AudioEvents.secondsPerBar))
        {
          CurrentState = CombatState.PRE_EXECUTION;
        }
        else
        {
          CurrentState = CombatState.DELAY_EXECUTION;
          lastBar = GlobalVariables.currentBar;
        }
        break;
      case CombatState.EXECUTION:
        beatmapManager.HideTrack();

        CurrentState = CombatState.HERO_ONE;
        break;
      default:
        break;
    }
  }

  public void GoPreviousTurn()
  {
    switch (CurrentState)
    {
      case CombatState.HERO_TWO:
        CurrentState = CombatState.HERO_ONE;
        break;
      case CombatState.HERO_THREE:
        CurrentState = CombatState.HERO_TWO;
        break;
      default:
        break;
    }
  }

  public void SubmitCommand(Command command)
  {
    switch (CurrentState)
    {
      case CombatState.HERO_ONE:
        submittedCommands[0] = command;
        break;
      case CombatState.HERO_TWO:
        submittedCommands[1] = command;
        break;
      case CombatState.HERO_THREE:
        submittedCommands[2] = command;
        break;
      default:
        break;
    }

    AdvanceState();
  }

  private int CompareCombatantSpeeds(Combatant x, Combatant y)
  {
    return y.Speed.CompareTo(x.Speed);
  }

  private void Lose()
  {
    CurrentState = CombatState.LOSE;
  }

  private void ReadHit(Combatant combatant, BeatmapManager.AccuracyGrade accuracyGrade)
  {
    if (combatant is Monster)
    {
      float damageMultiplier = 1f;
      switch (accuracyGrade)
      {
        case BeatmapManager.AccuracyGrade.PERFECT:
          damageMultiplier = 0.5f;
          break;
        case BeatmapManager.AccuracyGrade.GREAT:
          damageMultiplier = 0.7f;
          break;
        case BeatmapManager.AccuracyGrade.GOOD:
          damageMultiplier = 0.9f;
          break;
        default:
          break;
      }

      // For now, have the monster attack a random hero.
      int index = Random.Range(0, heroes.Length);
      heroes[index].DecreaseHealth(Mathf.RoundToInt(combatant.Attack * damageMultiplier));
    }
    else
    {
      // Combatant is a Hero.
      float damageMultiplier = 0f;
      switch (accuracyGrade)
      {
        case BeatmapManager.AccuracyGrade.PERFECT:
          damageMultiplier = 1f;
          break;
        case BeatmapManager.AccuracyGrade.GREAT:
          damageMultiplier = 0.66f;
          break;
        case BeatmapManager.AccuracyGrade.GOOD:
          damageMultiplier = 0.33f;
          break;
        default:
          break;
      }

      // For now, have the hero attack a random monster.
      int index = Random.Range(0, monsters.Length);
      monsters[index].DecreaseHealth(Mathf.RoundToInt(combatant.Attack * damageMultiplier));
    }
  }

  private void SortByInitiative()
  {
    Combatants = new List<Combatant>();
    Combatants.AddRange(heroes);
    Combatants.AddRange(monsters);
    Combatants.Sort(CompareCombatantSpeeds);
  }

  private float Threshold(float secondsPerBar)
  {
    return secondsPerBar / 2;
  }
}
