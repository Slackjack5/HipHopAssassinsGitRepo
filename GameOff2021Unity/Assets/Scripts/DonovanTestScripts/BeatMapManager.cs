using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapManager : MonoBehaviour
{
  [SerializeField] private GameObject track;
  [SerializeField] private GameObject beatCircle;
  [SerializeField] private Transform spawnerPos;
  [SerializeField] private Transform centerPos;
  [SerializeField] private Transform endPos;
  [Range(0.00f, 1.2f)] private float leniency = 0.07f;

  public readonly UnityEvent complete = new UnityEvent();

  private List<Note> notes = new List<Note>();
  private int nextSpawnIndex = 0;
  [SerializeField] private int nextHitIndex = 0;
  private float travelTime;
  private float damageMultiplier;
  private bool isGenerated = false;
  private bool isReady = false;

  private class Note
  {
    internal float time;
    internal GameObject beatCircle;
  }

  private void Start()
  {
    track.SetActive(false);
  }

  private void Update()
  {
    if (GlobalVariables.gameStarted) 
    { 
      //Initialize Variable
      travelTime = 2 * AudioEvents.secondsPerBeat;

      if (isGenerated)
      {
        //Spawn 
        if (nextSpawnIndex < notes.Count && AudioEvents.CurrentSegmentPosition >= notes[nextSpawnIndex].time - travelTime)
        {
          Spawn(nextSpawnIndex);
          nextSpawnIndex++;
        }

        if (isReady && Input.GetKeyDown("space"))
        {
          if (nextHitIndex < notes.Count)
          {
            float error = notes[nextHitIndex].time - AudioEvents.CurrentSegmentPosition;

            if (error >= -leniency / 3 && error <= leniency)
            {
              //Check to see where they hit exactly and give proper rating
              if (error <= leniency / 3) //Perfect Hit
              {
                damageMultiplier = 1;
              }
              else if (error <= (leniency / 3) * 2) //Great Hit
              {
                damageMultiplier = .66f;
              }
              else //Good Hit
              {
                damageMultiplier = .33f;
              }

              AkSoundEngine.PostEvent("Play_Cowbell", gameObject);

              Destroy(notes[nextHitIndex].beatCircle);
              nextHitIndex++;
            }
            else if (error > leniency) //Player Misses too early
            {
              Destroy(notes[nextHitIndex].beatCircle);
              nextHitIndex++;
              damageMultiplier = 0;
            }
            else if (error < -leniency / 3) //Player Misses too late
            {
              nextHitIndex++;
              damageMultiplier = 0;
            }
          }
        }

        if (nextHitIndex < notes.Count && AudioEvents.CurrentSegmentPosition - notes[nextHitIndex].time > leniency) //Player Presses Nothing
        {
          nextHitIndex++;
        }

        isReady = true;
      }
      else
      {
        isReady = false;
      }
    }
  }

  public void GenerateBeatmap(List<float[]> HitPointList, float startTime, float spBar)
  {
    for (int i = 0; i < HitPointList.Count; i++)
    {
      float[] pattern = HitPointList[i];
      for (int j = 0; j < pattern.Length; j++)
      {
        notes.Add(new Note() { time = pattern[j] + startTime + spBar * i });
      }
    }

    isGenerated = true;
  }

  public void HideTrack()
  {
    track.SetActive(false);
  }

  public void ShowTrack()
  {
    track.SetActive(true);
  }

  private void Spawn(int spawnIndex)
  {
    GameObject Circle = Instantiate(beatCircle, spawnerPos.position, Quaternion.identity);
    Circle.GetComponent<BeatCircle>().travelTime = travelTime;
    Circle.GetComponent<BeatCircle>().centerPos = centerPos;
    Circle.GetComponent<BeatCircle>().endPos = endPos;
    Circle.GetComponent<BeatCircle>().spawnerPos = spawnerPos;
    notes[spawnIndex].beatCircle = Circle;
  }
}
