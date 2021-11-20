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

  public static readonly UnityEvent<CombatState> onStateChange = new UnityEvent<CombatState>();

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
  public static List<Combatant> Combatants { get; private set; }

  public static CombatState CurrentState { get; private set; }

  public static List<Hero> Heroes { get; private set; }

  private void Awake()
  {
    Assert.IsTrue(beatmapManager, "beatmapManager is empty");
    Assert.IsTrue(monsterObjects, "monsterObjects is empty");

    beatmapManager.complete.AddListener(AdvanceState);
    beatmapManager.hit.AddListener(ReadHit);

    // Ensure heroes are ordered by ID.
    Heroes = heroObjects.GetComponentsInChildren<Hero>().ToList();
    var isSorted = true;
    for (var i = 0; i < Heroes.Count; i++)
    {
      if (Heroes[i].HeroId == i + 1) continue;
      Debug.LogWarning("Heroes are not ordered by ID! Will sort heroes.");
      isSorted = false;
    }

    if (!isSorted)
    {
      Heroes.Sort(CompareHeroIds);
    }

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
          foreach (Combatant combatant in Combatants)
          {
            combatant.SetInitialPosition();
          }

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
        // For now, each Monster will target a random Hero.
        SetRandomTargets();

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

    switch (state)
    {
      case CombatState.HeroOne:
        Heroes[0].Spotlight();
        Heroes[1].ResetPosition();
        Heroes[2].ResetPosition();
        break;
      case CombatState.HeroTwo:
        Heroes[0].ResetPosition();
        Heroes[1].Spotlight();
        Heroes[2].ResetPosition();
        break;
      case CombatState.HeroThree:
        Heroes[0].ResetPosition();
        Heroes[1].ResetPosition();
        Heroes[2].Spotlight();
        break;
      default:
        Heroes[0].ResetPosition();
        Heroes[1].ResetPosition();
        Heroes[2].ResetPosition();
        break;
    }

    onStateChange.Invoke(state);
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

  private void SetRandomTargets()
  {
    List<Hero> livingHeroes = Heroes.Where(hero => !hero.IsDead).ToList();
    List<Monster> livingMonsters = monsters.Where(monster => !monster.IsDead).ToList();

    foreach (Monster monster in livingMonsters)
    {
      int index = Random.Range(0, livingHeroes.Count);
      monster.SetTarget(livingHeroes[index]);
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

  private void ReadHit(BeatmapManager.Note note, BeatmapManager.AccuracyGrade accuracyGrade)
  {
    Combatant combatant = note.combatant;
    if (combatant.IsDead)
    {
      Debug.LogError("Read hit from a combatant that is dead!");
      return;
    }

    switch (combatant)
    {
      case Monster monster:
      {
        float damageMultiplier = accuracyGrade switch
        {
          BeatmapManager.AccuracyGrade.Perfect => 0f,
          BeatmapManager.AccuracyGrade.Great => 0.25f,
          BeatmapManager.AccuracyGrade.Good => 0.5f,
          _ => 1f
        };

        monster.DamageTarget(damageMultiplier, note.isLastOfCombatant);
        break;
      }
      case Hero hero:
      {
        Command command = GetHeroCommand(hero);
        if (command.CommandType != Command.Type.Attack && command.CommandType != Command.Type.Macro) return;

        float damageMultiplier = accuracyGrade switch
        {
          BeatmapManager.AccuracyGrade.Perfect => 1f,
          BeatmapManager.AccuracyGrade.Great => 0.5f,
          BeatmapManager.AccuracyGrade.Good => 0.25f,
          _ => 0f
        };

        hero.SetTarget(command.Target);
        hero.DamageTarget(damageMultiplier, note.isLastOfCombatant);
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

  private static int CompareHeroIds(Hero x, Hero y)
  {
    return y.HeroId.CompareTo(x.HeroId);
  }

  private static float Threshold(float secondsPerBar)
  {
    return secondsPerBar / 2;
  }
}
