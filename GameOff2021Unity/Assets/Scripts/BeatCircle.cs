using UnityEngine;

public class BeatCircle : MonoBehaviour
{
  public Transform centerPos;
  public Transform spawnerPos;
  public float travelTime;
  private float currentTime;
  public Transform endPos;
  private bool reachedMiddle;
  
  private void Start()
  {
    currentTime = travelTime;
  }

  private void Update()
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
      transform.position = reachedMiddle ? 
        Vector2.Lerp(endPos.position, centerPos.position, t) : 
        Vector2.Lerp(centerPos.position, spawnerPos.position, t);
    }
  }
}
