using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCircle : MonoBehaviour
{

  public Transform centerPos;
  public Transform spawnerPos;
  public float travelTime;
  private float currentTime;
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
      transform.position = Vector2.Lerp(centerPos.position, spawnerPos.position, t);
    }
}
