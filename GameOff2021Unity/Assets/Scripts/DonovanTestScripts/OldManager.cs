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
  public int previousState;
  private bool soundPlayed;


  //QTE Variables
  public GameObject QTERing;
  private int currentHitpoint = 0;
  private float time;
  private float newTime;
  public static int frames;

  //A way for us to visualize the Rhythm
  public GameObject rhythmDetector;
  //leaniancy
  [Range(0.00f,1.2f)]
  public float leaniancy = 1.2f;

  //Order of Rhythm Script:
  /*
   Step 1: Wait for player Input for a pattern and go to SawppingTurns State
   Step 2: Wait for QTE Rhythm State
   Step 3: Start Counting Time and Call the RhythmPatterns Script
   Step 4: Display When the player should hit in QuickTimeEvent()
  */

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
      if (gameplayStarted == false) { gameplayStarted = true; state = State.PlayerRhythm; spBeats = AudioEvents.secondsPerBeat; spBar = AudioEvents.secondsPerBar;}
      Debug.Log(frames);
    }
    switch (state)
    {
      case State.Waiting:
        previousState = 0;
        break;
      case State.QTERhythm:
        //Start counting from the beginning of the beat
        previousState = 1;
        CountTime();
        break;
      case State.PlayerRhythm: 
        //If Player Presses Space, Start Rhythm Pattern 1
        previousState = 2;
        if ((Input.GetKeyDown("1"))) //Do Pattern 1
        {
          state = State.SwappingTurns;
        }
        break;
      case State.SwappingTurns:
        if(GlobalVariables.currentBeat == 1 && frames == 0) //Wait till beat 1 , then swap states
        {
          if (previousState==2) { state = State.QTERhythm; QuickTimeEvent(RhythmPatterns.Pattern(spBeats, spBar, 1)); } else { state = State.PlayerRhythm; }
        }
        else if (GlobalVariables.currentBeat != 1 && frames == 0)
        {

        }
        break;
    }
  }

  public void QuickTimeEvent(float[] HitPoints)
  {
    if(currentHitpoint<HitPoints.Length) //Check to see if we are under the amount of hitpoints in this pattern
    {
      if (newTime >= HitPoints[currentHitpoint] && newTime <= HitPoints[currentHitpoint] + .01f) //If our current time reaches the middle of our hitregion, then play sound
      {
        //Play Sound Here
        if (soundPlayed == false) { AkSoundEngine.PostEvent("Play_Cowbell", gameObject); soundPlayed = true; }
      }

      if (HitPoints[0]==0)
      {
        QTERing.GetComponent<QTECircle>().StartQTE(leaniancy);
      }
      else
      {
        if (newTime >= HitPoints[currentHitpoint] - leaniancy) //Cehck to see if we are in the hitzone
        {
          //If so do this: This is where you check to see if the hit * time//
          rhythmDetector.GetComponent<SpriteRenderer>().color = Color.green; //Glow Green
                                                                             //Spawn Said Hit Point
                                                                             //
        }
        else
        {
          //If not  do this:
          rhythmDetector.GetComponent<SpriteRenderer>().color = Color.red; //Glow Red
        }

      }

      if (newTime >= HitPoints[currentHitpoint] + .01) //If our current time is above where we needed to hit then move to the next hit point
      {
        currentHitpoint += 1;
        soundPlayed = false;
      }

    }
    else //If we go through all the hitpoints, Swap turns
    {
      ResetVariables();
      state = State.SwappingTurns;
    }
  }

  public void ResetVariables()
  {
    currentHitpoint = 0;
    rhythmDetector.GetComponent<SpriteRenderer>().color = Color.red;
    soundPlayed = false;
    newTime = 0;
    time = 0;
  }

  public void CountTime()
  {
    time += Time.deltaTime;
    newTime = Mathf.Round(time * 10.0f) * 0.1f;
  }

  
 

}
