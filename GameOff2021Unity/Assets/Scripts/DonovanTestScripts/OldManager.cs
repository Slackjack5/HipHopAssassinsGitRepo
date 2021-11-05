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
  public float time;
  //A way for us to visualize the Rhythm
  public GameObject rhythmDetector;
  //leaniancy
  [Range(0.00f,0.50f)]
  public float leaniancy = 0.1f;


  private enum State
  {
    Waiting,
    QTERhythm,
    PlayerRhythm,
    SwappingTurns
  }
  // Start is called before the first frame update
  void Start()
  {
    //Wait Until the First Beat
    state = State.Waiting;

  }


  // Update is called once per frame
  void Update()
  {
    if (GlobalVariables.gameStarted == true)
    {
      if (gameplayStarted == false) { gameplayStarted = true; state = State.PlayerRhythm; spBeats = AudioEvents.secondsPerBeat; spBar = AudioEvents.secondsPerBar; } 
    }


    switch (state)
    {
      case State.Waiting:
        break;


      case State.QTERhythm:
        QuickTimeEvent(RhythmPatterns.Pattern(spBeats, spBar, 1));
        time += Time.deltaTime;
        break;



      case State.PlayerRhythm: ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //If Player Presses Space, Start Rhythm Pattern 1
        
        if((Input.GetKeyDown("1"))) //Do Pattern 1
        {
          state = State.SwappingTurns;
        }
        break;


      case State.SwappingTurns:
        if(GlobalVariables.currentBeat == 1) //Wait till beat 1 , then swap states
        {
          state = State.QTERhythm;
        }
        break;
    }
    
  }

  public void QuickTimeEvent(float[] HitPoints)
  {
    int currentHitpoint = 0;
    if(time >= HitPoints[currentHitpoint] + leaniancy) { currentHitpoint += 1; }
    if (time >= HitPoints[2] -leaniancy && time <= HitPoints[2] + leaniancy) 
    { 
      rhythmDetector.GetComponent<SpriteRenderer>().color = Color.green; 
    }
    else 
    { 
      rhythmDetector.GetComponent<SpriteRenderer>().color = Color.red; 
    }
    Debug.Log("CurrentHitPoint:" + currentHitpoint);
  }

  
 

}
