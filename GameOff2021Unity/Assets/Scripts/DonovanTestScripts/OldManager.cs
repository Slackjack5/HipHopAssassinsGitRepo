using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OldManager : MonoBehaviour
{
  //Variables
  private float spBeats;
  private float spBar;
  private bool gameplayStarted;
  private State state;

  //Time
  public float newTime;
  public static int frames;
  private int lastBar;
  public float startTime;
  private bool soundPlayed;


  //A way for us to visualize the Rhythm
  public GameObject rhythmDetector;

  public BeatMapManager BeatMapGen;

  private enum State
  {
    Waiting,
    QTERhythm,
    PlayerRhythm,
    SwappingTurns,
    DelayTurn
  }
  // Start is called before the first frame update
  void Start()
  {
    //Wait Until the First Beat
    state = State.Waiting;
  }

  private void FixedUpdate()
  {
    //Count the Amount of frames between each beat
    frames += 1;
  }

  // Update is called once per frame
  void Update()
  {
    //Wait till the first beat has started, before starting the game.
    if (GlobalVariables.gameStarted == true)
    {
      if (gameplayStarted == false) { gameplayStarted = true; state = State.PlayerRhythm; spBeats = AudioEvents.secondsPerBeat; spBar = AudioEvents.secondsPerBar;  }

      //Count Time Dumbass
      CountTime();
    }

    switch (state)
    {
      case State.Waiting:
        break;
      case State.QTERhythm:
        if (TimeCounter.totalTime >= startTime) { }
        break;
      case State.PlayerRhythm: 
        if ((Input.GetKeyDown("space"))) 
        {
          if(newTime<= Threshold(spBar))
          {
            state = State.SwappingTurns;
            Debug.Log("Coming from Player Turn");
          }
          else
          {
            state = State.DelayTurn;
            lastBar = GlobalVariables.currentBar;
          }
          
        }
        break;
      case State.SwappingTurns:
        //Generate List
        List<float[]> HitPointList= new List<float[]>();
        //Generate All Our Patterns
        HitPointList.Add(RhythmPatterns.Pattern(spBeats, spBar, 1));
        HitPointList.Add(RhythmPatterns.Pattern(spBeats, spBar, 2));
        HitPointList.Add(RhythmPatterns.Pattern(spBeats, spBar, 3));
        //Start Recording Time
        startTime = GlobalVariables.currentBar * spBar;
        BeatMapGen.GenerateBeat(HitPointList, startTime, spBar);
        state = State.QTERhythm;
        break;
      case State.DelayTurn:
        if(lastBar!=GlobalVariables.currentBar && (newTime >= Threshold(spBar)))
        {
          state = State.SwappingTurns;
          Debug.Log("Coming from Delay Turn");
        }
        break;
    }
  }

  public void CountTime()
  {
    newTime += Time.deltaTime;
  }

  public void ResetTime()
  {
    newTime = 0;
  }

  private float Threshold(float spBar)
  {
    return spBar / 2;
  }


}
