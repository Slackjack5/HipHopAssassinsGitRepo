using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class CombatManager : MonoBehaviour
{
  [SerializeField] private BeatmapManager beatmapManager;
  [SerializeField] private Hero[] heroes;
  [SerializeField] private GameObject monsterObjects;

  private bool isCombatDone;
  private int lastBar;
  private Monster[] monsters;
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
  public static CombatState CurrentState { get; private set; }
  public Hero[] Heroes => heroes;

  private void Awake()
  {
    Assert.IsTrue(beatmapManager, "beatmapManager is empty");
    Assert.IsTrue(monsterObjects, "monsterObjects is empty");
    
    beatmapManager.complete.AddListener(AdvanceState);
    beatmapManager.hit.AddListener(ReadHit);

    monsters = monsterObjects.GetComponentsInChildren<Monster>();

    SortByInitiative();

    foreach (Combatant combatant in Combatants)
    {
      combatant.dead.AddListener(() => beatmapManager.RemoveCombatantNotes(combatant));
    }
    
    CurrentState = CombatState.Start;
  }

  private void OnGUI()
  {
    for (var i = 0; i < monsters.Length; i++)
    {
      GUI.Label(new Rect(0, 30 * i, 200, 30),
        monsters[i].Name + " HP: " + monsters[i].CurrentHealth + " / " + monsters[i].MaxHealth);
    }
  }

  private void Update()
  {
    if (isCombatDone) return;

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
        if (lastBar != GlobalVariables.currentBar &&
            (AudioEvents.CurrentBarTime >= Threshold(AudioEvents.secondsPerBar)))
        {
          CurrentState = CombatState.PreExecution;
        }

        break;
      case CombatState.PreExecution:
        GeneratePatterns();

        CurrentState = CombatState.Execution;
        break;
    }
  }

  public void Lose()
  {
    CurrentState = CombatState.Lose;
    isCombatDone = true;
    beatmapManager.ForceFinish();
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

  private void AdvanceState()
  {
    switch (CurrentState)
    {
      case CombatState.HeroOne:
        CurrentState = heroes[1].IsDead ? CombatState.HeroThree : CombatState.HeroTwo;
        break;
      case CombatState.HeroTwo:
        if (heroes[2].IsDead)
        {
          DeterminePreExecutionState();
        }
        else
        {
          CurrentState = CombatState.HeroThree;
        }

        break;
      case CombatState.HeroThree:
        DeterminePreExecutionState();
        break;
      case CombatState.Execution:
        if (heroes[0].IsDead)
        {
          CurrentState = heroes[1].IsDead ? CombatState.HeroThree : CombatState.HeroTwo;
        }
        else
        {
          CurrentState = CombatState.HeroOne;
        }

        break;
    }
  }

  private bool AllHeroesDead()
  {
    return heroes.All(hero => hero.IsDead);
  }

  private bool AllMonstersDead()
  {
    return monsters.All(monster => monster.IsDead);
  }

  private void DeterminePreExecutionState()
  {
    if (AudioEvents.CurrentBarTime <= Threshold(AudioEvents.secondsPerBar))
    {
      CurrentState = CombatState.PreExecution;
    }
    else
    {
      CurrentState = CombatState.DelayExecution;
      lastBar = GlobalVariables.currentBar;
    }
  }

  private void GeneratePatterns()
  {
    var combatantPatterns = new Dictionary<Combatant, List<float>>();

    foreach (Combatant combatant in Combatants.Where(combatant => !combatant.IsDead))
    {
      if (combatant is Hero hero)
      {
        Command command = GetHeroCommand(hero);
        combatantPatterns[hero] = RhythmPatterns.Pattern(command.PatternId);
      }
      else
      {
        // Combatant is a Monster.
        combatantPatterns[combatant] = RhythmPatterns.Pattern(5);
      }
    }

    float executionStartTime = GlobalVariables.currentBar * AudioEvents.secondsPerBar;
    beatmapManager.GenerateBeatmap(combatantPatterns, executionStartTime);
  }

  private Command GetHeroCommand(Hero hero)
  {
    // HeroId is either 1, 2, or 3.
    return submittedCommands[hero.HeroId - 1];
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
      var hero = combatant as Hero;
      Command command = GetHeroCommand(hero);
      if (command.CommandType != Command.Type.Attack && command.CommandType != Command.Type.Macro) return;

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

  private void Win()
  {
    CurrentState = CombatState.Win;
    isCombatDone = true;
    beatmapManager.ForceFinish();
  }

  private static int CompareCombatantSpeeds(Combatant x, Combatant y)
  {
    return y.Speed.CompareTo(x.Speed);
  }

  private static float Threshold(float secondsPerBar)
  {
    return secondsPerBar / 2;
  }
}
