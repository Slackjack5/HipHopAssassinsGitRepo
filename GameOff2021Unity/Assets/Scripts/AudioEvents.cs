using UnityEngine;
using UnityEngine.Events;

public class AudioEvents : MonoBehaviour
{
  //Wwise
  public AK.Wwise.Event rhythmHeckinEvent;
  public UnityEvent OnLevelEnded;
  public static float secondsPerBeat;
  public static float secondsPerBar;
  public static float bpm;
  //Unity Events
  public UnityEvent OnEveryGrid;
  public UnityEvent OnEveryBeat;
  public UnityEvent OnEveryBar;
  public UnityEvent OnSubtractTime;
  //Functions
  public int GridCount = 0;
  public int gridCounter = 0;
  public bool startCounting = false;
  //Timing
  public int currentBar= GlobalVariables.currentBar;
  public int currentBeat= GlobalVariables.currentBeat;
  public int currentGrid= GlobalVariables.currentGrid;
  public bool gameStarted = GlobalVariables.fightStarted;

  private static AkSegmentInfo currentSegment;
  private static int currentBarStartTime;  // The time, in milliseconds, that the current bar started at

  //id of the wwise event - using this to get the playback position
  static uint playingID;

  /// <summary>
  /// The time elapsed, in seconds, since the current bar started
  /// </summary>
  public static float CurrentBarTime =>
    // iCurrentPosition is in milliseconds.
    (float)(currentSegment.iCurrentPosition - currentBarStartTime) / 1000;

  /// <summary>
  /// The time elapsed, in seconds, since the segment started
  /// </summary>
  public static float CurrentSegmentPosition =>
    // iCurrentPosition is in milliseconds.
    (float)currentSegment.iCurrentPosition / 1000;

  private void Start()
  {
    playingID = rhythmHeckinEvent.Post(gameObject, (uint)(AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition), MusicCallbackFunction);
    currentSegment = new AkSegmentInfo();
    GlobalVariables.fightStarted = false;
  }

  private void Update()
  {
    currentBar = GlobalVariables.currentBar;
    currentBeat = GlobalVariables.currentBeat;
    currentGrid = GlobalVariables.currentGrid;

    AkSoundEngine.GetPlayingSegmentInfo(playingID, currentSegment);
  }

  void MusicCallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
  {

    AkMusicSyncCallbackInfo _musicInfo = (AkMusicSyncCallbackInfo)in_info;

    switch (_musicInfo.musicSyncType)
    {
      case AkCallbackType.AK_MusicSyncUserCue:

        CustomCues(_musicInfo.userCueName, _musicInfo);

        secondsPerBeat = _musicInfo.segmentInfo_fBeatDuration;
        secondsPerBar = _musicInfo.segmentInfo_fBarDuration;
        bpm = _musicInfo.segmentInfo_fBeatDuration * 60f;
        break;
      case AkCallbackType.AK_MusicSyncBeat:


        OnEveryBeat.Invoke();
        break;
      case AkCallbackType.AK_MusicSyncBar:
        //I want to make sure that the secondsPerBeat is defined on our first measure.
        if (GlobalVariables.fightStarted == false)
        {
          // If the game hasn't started yet, start it on beat 1
          GlobalVariables.fightStarted = true;
        }

        secondsPerBeat = _musicInfo.segmentInfo_fBeatDuration;
        secondsPerBar = _musicInfo.segmentInfo_fBarDuration;

        currentBarStartTime = currentSegment.iCurrentPosition;

        OnEveryBar.Invoke();
        break;

      case AkCallbackType.AK_MusicSyncGrid:
        OnEveryGrid.Invoke();
        break;
      default:
        break;

    }

  }

  public void IncreaseBar()
  {
    GlobalVariables.currentBar += 1;
  }

  public void IncreaseBeat()
  {
    if (GlobalVariables.currentBeat < 4)//Insert Time Signature
    {
      GlobalVariables.currentBeat += 1;
    }
    else
    {
      GlobalVariables.currentBeat = 1;
    }
  }
  
  public void IncreaseGrid()
  {
    if (GlobalVariables.currentGrid < 4)//Insert Time Signature
    {
      GlobalVariables.currentGrid += 1;
    }
    else
    {
      GlobalVariables.currentGrid = 1;
    }

  }


  public void CustomCues(string cueName, AkMusicSyncCallbackInfo _musicInfo)
  {
    switch (cueName)
    {
      case "Example Case":
        break;
      default:
        break;
    }
  }

  public void Bump()
  {
    //Camera Shake

  }

}

public static class GlobalVariables
{
  public static int currentBar;
  public static int currentBeat;
  public static int currentGrid;
  public static bool fightStarted;
}
