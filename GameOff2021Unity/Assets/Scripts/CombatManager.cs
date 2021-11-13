using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    Unspecified, 
    Start,
    HeroOne, 
    HeroTwo,
    HeroThree,
    DelayExecution,
    PreExecution,
    Execution, 
    Win,
    Lose
  }

  /// <summary>
  /// Combatants sorted in initiative order
  /// </summary>
  public List<Combatant> Combatants { get; private set; }
  public CombatState CurrentState { get; private set; }
  public Hero[] Heroes => heroes;

  private void Awake()
  {
    beatmapManager.complete.AddListener(AdvanceState);
    beatmapManager.hit.AddListener(ReadHit);
    timer.expire.AddListener(Lose);

    SortByInitiative();

    CurrentState = CombatState.Start;
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
    if (AllMonstersDead())
    {
      Win();
    }

    if (AllHeroesDead())
    {
      Lose();
    }

    switch (CurrentState)
    {
      case CombatState.Start:
        if (GlobalVariables.fightStarted)
        {
          CurrentState = CombatState.HeroOne;
        }
        
        break;
      case CombatState.DelayExecution:
        if (lastBar != GlobalVariables.currentBar && (AudioEvents.CurrentBarTime >= Threshold(AudioEvents.secondsPerBar)))
        {
          CurrentState = CombatState.PreExecution;
        }
        break;
      case CombatState.PreExecution:
        //Generate All Our Patterns
        Dictionary<Combatant, List<float>> combatantPatterns = new Dictionary<Combatant, List<float>>();

        foreach (Combatant combatant in Combatants)
        {
          if (combatant is Hero hero)
          {
            // HeroId is either 1, 2, or 3.
            Command command = GetHeroCommand(hero);
            combatantPatterns[hero] = RhythmPatterns.Pattern(command.PatternId);
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

        CurrentState = CombatState.Execution;
        break;
      case CombatState.Unspecified:
      case CombatState.HeroOne:
      case CombatState.HeroTwo:
      case CombatState.HeroThree:
      case CombatState.Execution:
      case CombatState.Win:
      case CombatState.Lose:
      default:
        break;
    }
  }

  private void AdvanceState()
  {
    switch (CurrentState)
    {
      case CombatState.HeroOne:
        CurrentState = CombatState.HeroTwo;
        break;
      case CombatState.HeroTwo:
        CurrentState = CombatState.HeroThree;
        break;
      case CombatState.HeroThree:
        beatmapManager.ShowTrack();

        if (AudioEvents.CurrentBarTime <= Threshold(AudioEvents.secondsPerBar))
        {
          CurrentState = CombatState.PreExecution;
        }
        else
        {
          CurrentState = CombatState.DelayExecution;
          lastBar = GlobalVariables.currentBar;
        }
        break;
      case CombatState.Execution:
        beatmapManager.HideTrack();

        CurrentState = CombatState.HeroOne;
        break;
    }
  }

  public void SubmitCommand(Command command)
  {
    switch (CurrentState)
    {
      case CombatState.HeroOne:
        submittedCommands[0] = command;
        break;
      case CombatState.HeroTwo:
        submittedCommands[1] = command;
        break;
      case CombatState.HeroThree:
        submittedCommands[2] = command;
        break;
    }

    AdvanceState();
  }

  private bool AllHeroesDead()
  {
    return heroes.All(hero => hero.IsDead);
  }

  private bool AllMonstersDead()
  {
    return monsters.All(monster => monster.IsDead);
  }

  private static int CompareCombatantSpeeds(Combatant x, Combatant y)
  {
    return y.Speed.CompareTo(x.Speed);
  }

  private Command GetHeroCommand(Hero hero)
  {
    // HeroId is either 1, 2, or 3.
    return submittedCommands[hero.HeroId - 1];
  }

  private void Lose()
  {
    CurrentState = CombatState.Lose;
  }

  private void ReadHit(Combatant combatant, BeatmapManager.AccuracyGrade accuracyGrade)
  {
    if (combatant is Monster)
    {
      var damageMultiplier = 1f;
      switch (accuracyGrade)
      {
        case BeatmapManager.AccuracyGrade.Perfect:
          damageMultiplier = 0.5f;
          break;
        case BeatmapManager.AccuracyGrade.Great:
          damageMultiplier = 0.7f;
          break;
        case BeatmapManager.AccuracyGrade.Good:
          damageMultiplier = 0.9f;
          break;
      }

      // For now, have the monster attack a random hero.
      int index = Random.Range(0, heroes.Length);
      heroes[index].DecreaseHealth(Mathf.RoundToInt(combatant.Attack * damageMultiplier));
    }
    else
    {
      // Combatant is a Hero.
      var damageMultiplier = 0f;
      switch (accuracyGrade)
      {
        case BeatmapManager.AccuracyGrade.Perfect:
          damageMultiplier = 1f;
          break;
        case BeatmapManager.AccuracyGrade.Great:
          damageMultiplier = 0.66f;
          break;
        case BeatmapManager.AccuracyGrade.Good:
          damageMultiplier = 0.33f;
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

  private static float Threshold(float secondsPerBar)
  {
    return secondsPerBar / 2;
  }

  private void Win()
  {
    CurrentState = CombatState.Win;
  }
}
