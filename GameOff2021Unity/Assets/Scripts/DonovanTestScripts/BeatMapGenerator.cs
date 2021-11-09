using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMapGenerator : MonoBehaviour
{

  private List<float> absoluteHitPointList = new List<float>();
  private int nextSpawnIndex=0;
  public GameObject BeatCircle;
  public Transform spawnerPos;
  public Transform centerPos;
  public float travelTime;
  private void Update()
  {
   
    if (GlobalVariables.gameStarted) 
    { 
      //Initialize Variable
      travelTime = 2 * AudioEvents.secondsPerBeat; 
      //Spawn 
      if (nextSpawnIndex<absoluteHitPointList.Count && TimeCounter.totalTime >= absoluteHitPointList[nextSpawnIndex]-travelTime)
      {
        Spawn();
        nextSpawnIndex++;
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
        absoluteHitPointList.Add(pattern[j] + startTime + spBar * i);
      }
    }

    for (int i = 0; i < absoluteHitPointList.Count; i++)
    {
      Debug.Log("This is Point: " + i + " : " + absoluteHitPointList[i]);
    }
    //Debug.Log(absoluteHitPointList);
  }

  private void Spawn()
  {
    GameObject Circle = Instantiate(BeatCircle, spawnerPos.position, Quaternion.identity);
    Circle.GetComponent<BeatCircle>().travelTime = travelTime;
    Circle.GetComponent<BeatCircle>().centerPos = centerPos;
    Circle.GetComponent<BeatCircle>().spawnerPos = spawnerPos;
  }
}
