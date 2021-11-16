using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class CombatManager : MonoBehaviour
{
  [SerializeField] private float startStateDuration;
  [SerializeField] private BeatmapManager beatmapManager;
  [SerializeField] private GameObject heroObjects;
  [SerializeField] private GameObject monsterObjects;

  public readonly UnityEvent<CombatState> onChangeState = new UnityEvent<CombatState>();

  private float currentStartStateTime;
  private bool isCombatDone;
  private bool isStarting;
  private int lastBar;
  private Monster[] monsters;
  private readonly Command[] submittedCommands = new Command[3];

  public enum CombatState
  {
    Unspecified,
    PreStart,
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

  public Hero[] Heroes => heroObjects.GetComponentsInChildren<Hero>();

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

    ChangeState(CombatState.PreStart);
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
        if (!isStarting)
        {
          currentStartStateTime = startStateDuration;
          isStarting = true;
        }

        currentStartStateTime -= Time.deltaTime;

        if (currentStartStateTime <= 0)
        {
          ChangeState(CombatState.HeroOne);
        }

        break;
      case CombatState.DelayExecution:
        if (lastBar != GlobalVariables.currentBar &&
            (AudioEvents.CurrentBarTime >= Threshold(AudioEvents.secondsPerBar)))
        {
          ChangeState(CombatState.PreExecution);
        }

        break;
      case CombatState.PreExecution:
        GeneratePatterns();

        ChangeState(CombatState.Execution);
        break;
    }
  }

  public void Lose()
  {
    ChangeState(CombatState.Lose);
    isCombatDone = true;
    beatmapManager.ForceFinish();
  }

  public void StartFight()
  {
    ChangeState(CombatState.Start);
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
        ChangeState(Heroes[1].IsDead ? CombatState.HeroThree : CombatState.HeroTwo);
        break;
      case CombatState.HeroTwo:
        if (Heroes[2].IsDead)
        {
          DeterminePreExecutionState();
        }
        else
        {
          ChangeState(CombatState.HeroThree);
        }

        break;
      case CombatState.HeroThree:
        DeterminePreExecutionState();
        break;
      case CombatState.Execution:
        if (Heroes[0].IsDead)
        {
          ChangeState(Heroes[1].IsDead ? CombatState.HeroThree : CombatState.HeroTwo);
        }
        else
        {
          ChangeState(CombatState.HeroOne);
        }

        break;
    }
  }

  private bool AllHeroesDead()
  {
    return Heroes.All(hero => hero.IsDead);
  }

  private bool AllMonstersDead()
  {
    return monsters.All(monster => monster.IsDead);
  }

  private void ChangeState(CombatState state)
  {
    CurrentState = state;
    onChangeState.Invoke(state);
  }

  private void DeterminePreExecutionState()
  {
    if (AudioEvents.CurrentBarTime <= Threshold(AudioEvents.secondsPerBar))
    {
      ChangeState(CombatState.PreExecution);
    }
    else
    {
      ChangeState(CombatState.DelayExecution);
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
    switch (combatant)
    {
      case Monster monster:
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
        List<Hero> livingHeroes = Heroes.Where(hero => !hero.IsDead).ToList();
        int index = Random.Range(0, livingHeroes.Count);
        monster.SetTarget(livingHeroes[index]);
        monster.DamageTarget(damageMultiplier);
        break;
      }
      case Hero hero:
      {
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
        List<Monster> livingMonsters = monsters.Where(monster => !monster.IsDead).ToList();
        int index = Random.Range(0, livingMonsters.Count);
        hero.SetTarget(livingMonsters[index]);
        hero.DamageTarget(damageMultiplier);
        break;
      }
      default:
        Debug.LogError("The combatant is neither a Hero nor a Monster. ReadHit failed.");
        break;
    }
  }

  private void SortByInitiative()
  {
    Combatants = new List<Combatant>();
    Combatants.AddRange(Heroes);
    Combatants.AddRange(monsters);
    Combatants.Sort(CompareCombatantSpeeds);
  }

  private void Win()
  {
    ChangeState(CombatState.Win);
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
