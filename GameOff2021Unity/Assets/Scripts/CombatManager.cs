using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
  [SerializeField] private BeatmapManager beatmapManager;
  [SerializeField] private Timer timer;

  private float executionStartTime;
  private int lastBar;
  private readonly Command[] submittedCommands = new Command[3];

  public enum CombatState
  {
    UNSPECIFIED, START, HERO_ONE, HERO_TWO, HERO_THREE, DELAY_EXECUTION, PRE_EXECUTION, EXECUTION, WIN, LOSE
  }

  public CombatState CurrentState { get; private set; }

  private void Start()
  {
    beatmapManager.complete.AddListener(AdvanceState);
    timer.expire.AddListener(Lose);

    CurrentState = CombatState.START;
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
        List<float[]> patterns = new List<float[]>();
        foreach (Command command in submittedCommands)
        {
          patterns.Add(RhythmPatterns.Pattern(command.PatternId));
        }

        //Start Recording Time
        executionStartTime = GlobalVariables.currentBar * AudioEvents.secondsPerBar;
        beatmapManager.GenerateBeatmap(patterns, executionStartTime);

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

  private void Lose()
  {
    CurrentState = CombatState.LOSE;
  }

  private float Threshold(float secondsPerBar)
  {
    return secondsPerBar / 2;
  }
}
