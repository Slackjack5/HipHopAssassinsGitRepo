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

  public static readonly UnityEvent<State> onStateChange = new UnityEvent<State>();

  private float currentStartStateTime;
  private bool isStarting;
  private int lastBar;
  private readonly Command[] submittedCommands = new Command[3];

  public enum State
  {
    Unspecified,
    Inactive,
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

  public static State CurrentState { get; private set; }

  public static List<Hero> Heroes { get; private set; }

  public static List<Monster> Monsters { get; private set; }

  private void Awake()
  {
    Assert.IsTrue(beatmapManager, "beatmapManager is empty");

    beatmapManager.complete.AddListener(AdvanceState);
    beatmapManager.hit.AddListener(ReadHit);

    Heroes = heroObjects.GetComponentsInChildren<Hero>().ToList();
    Assert.IsTrue(Heroes.Count == 3, "Heroes.Count != 3");

    // Ensure heroes are ordered by ID.
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

    Timer.onExpire.AddListener(Lose);

    CurrentState = State.Inactive;
  }

  private void Update()
  {
    Debug.Log($"Current state is {CurrentState}");

    if (CurrentState == State.Inactive || CurrentState == State.Win || CurrentState == State.Lose) return;

    if (CurrentState != State.PreStart && CurrentState != State.Start)
    {
      if (AllMonstersDead())
      {
        Win();
      }

      if (AllHeroesDead())
      {
        Lose();
      }
    }

    switch (CurrentState)
    {
      case State.Start:
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
          ChangeState(State.HeroOne);
        }

        break;
      case State.DelayExecution:
        if (lastBar != GlobalVariables.currentBar &&
            AudioEvents.CurrentBarTime >= Threshold(AudioEvents.secondsPerBar))
        {
          ChangeState(State.PreExecution);
        }

        break;
      case State.PreExecution:
        // For now, each Monster will target a random Hero.
        SetRandomTargets();

        GeneratePatterns();

        ChangeState(State.Execution);
        break;
    }
  }

  private void OnGUI()
  {
    if (Monsters == null) return;

    for (var i = 0; i < Monsters.Count; i++)
    {
      GUI.Label(new Rect(0, 30 * i, 200, 30),
        Monsters[i].Name + " HP: " + Monsters[i].CurrentHealth + " / " + Monsters[i].MaxHealth);
    }
  }

  public void Begin(Encounter encounter)
  {
    Monsters = encounter.GetComponentsInChildren<Monster>().ToList();

    SortByInitiative();

    foreach (Combatant combatant in Combatants)
    {
      combatant.dead.AddListener(() => beatmapManager.RemoveCombatantNotes(combatant));
    }

    encounter.GetComponent<CombatantsMovement>().onComplete.AddListener(StartFight);

    ChangeState(State.PreStart);
  }

  private void StartFight()
  {
    Timer.Activate();
    ChangeState(State.Start);
  }

  private void Lose()
  {
    ChangeState(State.Lose);
    beatmapManager.ForceFinish();
  }

  public void SubmitCommand(Command command)
  {
    switch (CurrentState)
    {
      case State.HeroOne:
        submittedCommands[0] = command;
        break;
      case State.HeroTwo:
        submittedCommands[1] = command;
        break;
      case State.HeroThree:
        submittedCommands[2] = command;
        break;
    }

    AdvanceState();
  }

  private void AdvanceState()
  {
    switch (CurrentState)
    {
      case State.HeroOne:
        ChangeState(Heroes[1].IsDead ? State.HeroThree : State.HeroTwo);
        break;
      case State.HeroTwo:
        if (Heroes[2].IsDead)
        {
          DeterminePreExecutionState();
        }
        else
        {
          ChangeState(State.HeroThree);
        }

        break;
      case State.HeroThree:
        DeterminePreExecutionState();
        break;
      case State.Execution:
        if (Heroes[0].IsDead)
        {
          ChangeState(Heroes[1].IsDead ? State.HeroThree : State.HeroTwo);
        }
        else
        {
          ChangeState(State.HeroOne);
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
    return Monsters.All(monster => monster.IsDead);
  }

  private static void ChangeState(State state)
  {
    CurrentState = state;

    switch (state)
    {
      case State.HeroOne:
        Heroes[0].Spotlight();
        Heroes[1].ResetPosition();
        Heroes[2].ResetPosition();
        break;
      case State.HeroTwo:
        Heroes[0].ResetPosition();
        Heroes[1].Spotlight();
        Heroes[2].ResetPosition();
        break;
      case State.HeroThree:
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
      ChangeState(State.PreExecution);
    }
    else
    {
      ChangeState(State.DelayExecution);
      lastBar = GlobalVariables.currentBar;
    }
  }

  private void SetRandomTargets()
  {
    List<Hero> livingHeroes = Heroes.Where(hero => !hero.IsDead).ToList();
    List<Monster> livingMonsters = Monsters.Where(monster => !monster.IsDead).ToList();

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
        combatantPatterns[hero] = RhythmPatterns.Pattern(command.patternId);
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
        float effectMultiplier = accuracyGrade switch
        {
          BeatmapManager.AccuracyGrade.Perfect => 1f,
          BeatmapManager.AccuracyGrade.Great => 0.5f,
          BeatmapManager.AccuracyGrade.Good => 0.25f,
          _ => 0f
        };

        Command command = GetHeroCommand(hero);
        switch (command)
        {
          case Macro macro:
            macro.Execute(hero, effectMultiplier, note.isLastOfCombatant);
            break;
          case Attack attack:
            attack.Execute(hero, effectMultiplier, note.isLastOfCombatant);
            break;
          case Consumable consumable:
            consumable.Execute(hero);
            break;
        }

        break;
      }
      default:
        Debug.LogError("The combatant is neither a Hero nor a Monster. ReadHit failed.");
        break;
    }
  }

  private static void SortByInitiative()
  {
    Combatants = new List<Combatant>();
    Combatants.AddRange(Heroes);
    Combatants.AddRange(Monsters);
    Combatants.Sort(CompareCombatantSpeeds);
  }

  private void Win()
  {
    ChangeState(State.Win);
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
