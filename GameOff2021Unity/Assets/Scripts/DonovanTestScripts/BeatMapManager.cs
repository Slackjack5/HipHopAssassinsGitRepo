using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapManager : MonoBehaviour
{
  [SerializeField] private GameObject track;

  public readonly UnityEvent complete = new UnityEvent();

  private List<HitPoint> absoluteHitPointList = new List<HitPoint>();
  private int nextSpawnIndex=0;
  private int nextHitIndex = 0;
  public GameObject BeatCircle;
  public Transform spawnerPos;
  public Transform centerPos;
  public Transform endPos;
  public float travelTime;

  //leaniancy
  [Range(0.00f, 1.2f)]
  public float leaniancy = 1.2f;
  public float damageMultiplier;

  private bool isGenerated = false;

  private class HitPoint
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
        if (nextSpawnIndex < absoluteHitPointList.Count && TimeCounter.TotalTime >= absoluteHitPointList[nextSpawnIndex].time - travelTime)
        {
          Spawn(nextSpawnIndex);
          nextSpawnIndex++;
        }

        if (Input.GetKeyDown("space"))
        {
          if (nextHitIndex < absoluteHitPointList.Count)
          {
            float error = absoluteHitPointList[nextHitIndex].time - TimeCounter.TotalTime;

            if (error <= leaniancy)
            {
              //Check to see where they hit exactly and give proper rating
              if (error < leaniancy / 3) //Perfect Hit
              {
                damageMultiplier = 1;
                Debug.Log("Perfect Hit!");
              }
              else if (error < (leaniancy / 3) * 2) //Great Hit
              {
                damageMultiplier = .66f;
                Debug.Log("Great Hit!");
              }
              else //Good Hit
              {
                damageMultiplier = .33f;
                Debug.Log("Good Hit!");
              }

              AkSoundEngine.PostEvent("Play_Cowbell", gameObject);

              //Destroy the Beat
              Destroy(absoluteHitPointList[nextHitIndex].beatCircle);
              nextHitIndex++;
            }
            else if (error > leaniancy) //Player Misses too early
            {
              nextHitIndex++;
              damageMultiplier = 0;
              Debug.Log("Too Early");
            }
            else if (TimeCounter.TotalTime - absoluteHitPointList[nextHitIndex].time <= leaniancy) //Player Misses too late
            {
              nextHitIndex++;
              damageMultiplier = 0;
              Debug.Log("Too Late");
            }
          }
        }

        if (nextHitIndex < absoluteHitPointList.Count && TimeCounter.TotalTime - absoluteHitPointList[nextHitIndex].time > leaniancy) //Player Presses Nothing
        {
          nextHitIndex++;
          Debug.Log("Press a damn button");
        }
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
        absoluteHitPointList.Add(new HitPoint() { time = pattern[j] + startTime + spBar * i });
      }
    }

    foreach (HitPoint point in absoluteHitPointList)
    {
      print("Point time: " + point.time);
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
    GameObject Circle = Instantiate(BeatCircle, spawnerPos.position, Quaternion.identity);
    Circle.GetComponent<BeatCircle>().travelTime = travelTime;
    Circle.GetComponent<BeatCircle>().centerPos = centerPos;
    Circle.GetComponent<BeatCircle>().endPos = endPos;
    Circle.GetComponent<BeatCircle>().spawnerPos = spawnerPos;
    absoluteHitPointList[spawnIndex].beatCircle = Circle;
  }
}
