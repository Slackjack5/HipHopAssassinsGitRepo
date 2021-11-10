using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCircle : MonoBehaviour
{

  public Transform centerPos;
  public Transform spawnerPos;
  public float travelTime;
  private float currentTime;
  public Transform endPos;
  private bool reachedMiddle;
  // Start is called before the first frame update
  void Start()
  {
  currentTime = travelTime;
  }

  // Update is called once per frame
  void Update()
  {
    currentTime -= Time.deltaTime;
    float t = Mathf.InverseLerp(0, travelTime, currentTime);
    if(t<=0)
    {
      currentTime = travelTime;
      if(reachedMiddle)
      {
        Destroy(gameObject);
      }
      else
      {
        reachedMiddle = true;
      }
      
    }
    else
    {
      if (reachedMiddle) 
      { 
        transform.position = Vector2.Lerp(endPos.position, centerPos.position, t); 
      } 
      else 
      { 
        transform.position = Vector2.Lerp(centerPos.position, spawnerPos.position, t); 
      }
    }
      
  }
}
