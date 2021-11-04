using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OldManager : MonoBehaviour
{
  //Variables
  private float spBeats;
  private float spBar;
  public bool gameplayStarted;
  private State state;
  //A way for us to visualize the Rhythm
  public GameObject rhythmDetector;
  //leaniancy
  [Range(0.00f,0.50f)]
  public float leaniancy = 0.1f;


  private enum State
  {
    Waiting,
    EnemyRhythm,
    PlayerRhythm,
    Scoring
  }
  // Start is called before the first frame update
  void Start()
  {
    //Wait Until the First Beat
    state = State.Waiting;
    spBeats = AudioEvents.secondsPerBeat;
    spBar = AudioEvents.secondsPerBar;
  }


  // Update is called once per frame
  void Update()
  {
    if (GlobalVariables.gameStarted == true)
    {
      if (gameplayStarted == false) { gameplayStarted=true; state = State.PlayerRhythm; }
    }


    switch (state)
    {
      case State.Waiting:
        break;
      case State.EnemyRhythm:

          break;
      case State.PlayerRhythm: ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //If Player Presses Space, Start Rhythm Pattern 1
        break;
    }
    
  }

  public void OnDebug()
  {
    Debug.Log("space key was pressed");
    //for(int i;i<)
    Debug.Log(RhythmPatterns.Pattern1(spBeats, spBar));
    return;
  }
  
 

}
