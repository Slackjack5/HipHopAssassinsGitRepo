using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMapManager : MonoBehaviour
{

  private List<HitPoint> absoluteHitPointList = new List<HitPoint>();
  private int nextSpawnIndex=0;
  public int nextHitIndex = 0;
  public GameObject BeatCircle;
  public Transform spawnerPos;
  public Transform centerPos;
  public Transform endPos;
  public float travelTime;

  //leaniancy
  [Range(0.00f, 1.2f)]
  public float leaniancy = 1.2f;

  private class HitPoint
  {
    internal float time;
    internal GameObject beatCircle;
  }

  private void Update()
  {
   
    if (GlobalVariables.gameStarted) 
    { 
      //Initialize Variable
      travelTime = 2 * AudioEvents.secondsPerBeat; 
      //Spawn 
      if (nextSpawnIndex<absoluteHitPointList.Count && TimeCounter.totalTime >= absoluteHitPointList[nextSpawnIndex].time-travelTime)
      {
        Spawn(nextSpawnIndex);
        nextSpawnIndex++;
      }

      if((Input.GetKeyDown("space")))
      {
        if (nextHitIndex<absoluteHitPointList.Count && absoluteHitPointList[nextHitIndex].time-TimeCounter.totalTime<=leaniancy)
        {
          //Destroy the Beat
          Destroy(absoluteHitPointList[nextHitIndex].beatCircle);
          nextHitIndex++;
        }
        else if(nextHitIndex < absoluteHitPointList.Count && absoluteHitPointList[nextHitIndex].time - TimeCounter.totalTime > leaniancy)
        {
          nextHitIndex++;
        }

      }

      if (nextHitIndex < absoluteHitPointList.Count && TimeCounter.totalTime - absoluteHitPointList[nextHitIndex].time > leaniancy)
      {
        nextHitIndex++;
      }
    }

  }

  public void GenerateBeat(List<float[]> HitPointList, float startTime, float spBar)
  {
    

    for (int i = 0; i < HitPointList.Count; i++)
    {
      float[] pattern = HitPointList[i];
      for (int j = 0; j < pattern.Length; j++)
      {
        absoluteHitPointList.Add(new HitPoint() { time = pattern[j] + startTime + spBar * i });
      }
    }

    for (int i = 0; i < absoluteHitPointList.Count; i++)
    {
      Debug.Log("This is Point: " + i + " : " + absoluteHitPointList[i]);
    }
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
