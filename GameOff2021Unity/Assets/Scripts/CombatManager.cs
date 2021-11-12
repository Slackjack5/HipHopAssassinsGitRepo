using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
  [SerializeField] private BeatmapManager beatmapManager;
  [SerializeField] private MenuManager menuManager;

  private float executionStartTime;
  private int lastBar;

  public enum CombatState
  {
    START, HERO_ONE, HERO_TWO, HERO_THREE, DELAY_EXECUTION, PRE_EXECUTION, EXECUTION
  }

  public CombatState CurrentState { get; private set; }

  private void Start()
  {
    beatmapManager.complete.AddListener(AdvanceState);
    menuManager.HideMenu();

    CurrentState = CombatState.START;
  }

  private void Update()
  {
    switch (CurrentState)
    {
      case CombatState.START:
        if (GlobalVariables.gameStarted)
        {
          menuManager.OpenTopMenu();
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
        //Generate List
        List<float[]> HitPointList = new List<float[]>();
        //Generate All Our Patterns
        HitPointList.Add(RhythmPatterns.Pattern(AudioEvents.secondsPerBeat, AudioEvents.secondsPerBar, 1));
        HitPointList.Add(RhythmPatterns.Pattern(AudioEvents.secondsPerBeat, AudioEvents.secondsPerBar, 2));
        HitPointList.Add(RhythmPatterns.Pattern(AudioEvents.secondsPerBeat, AudioEvents.secondsPerBar, 3));
        //Start Recording Time
        executionStartTime = GlobalVariables.currentBar * AudioEvents.secondsPerBar;
        beatmapManager.GenerateBeatmap(HitPointList, executionStartTime, AudioEvents.secondsPerBar);
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
        menuManager.OpenTopMenu();

        CurrentState = CombatState.HERO_TWO;
        break;
      case CombatState.HERO_TWO:
        menuManager.OpenTopMenu();

        CurrentState = CombatState.HERO_THREE;
        break;
      case CombatState.HERO_THREE:
        menuManager.HideMenu();
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
        menuManager.OpenTopMenu();
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

  private float Threshold(float secondsPerBar)
  {
    return secondsPerBar / 2;
  }
}
