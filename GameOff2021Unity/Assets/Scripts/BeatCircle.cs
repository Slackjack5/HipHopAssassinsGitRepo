using UnityEngine;

public class BeatCircle : MonoBehaviour
{
  [HideInInspector] public Transform spawnerPos;
  [HideInInspector] public Transform centerPos;
  [HideInInspector] public Transform endPos;
  [HideInInspector] public float travelTime;

  private float currentTime;
  private bool reachedMiddle;

  private void Start()
  {
    currentTime = travelTime;
  }

  private void Update()
  {
    currentTime -= Time.deltaTime;
    float t = Mathf.InverseLerp(0, travelTime, currentTime);
    if (t <= 0)
    {
      currentTime = travelTime;
      if (reachedMiddle)
      {
        // Reached the end of the track, so destroy this object.
        Destroy(gameObject);
      }
      else
      {
        reachedMiddle = true;
      }
    }
    else
    {
      transform.position = reachedMiddle
        ? Vector2.Lerp(endPos.position, centerPos.position, t)
        : Vector2.Lerp(centerPos.position, spawnerPos.position, t);
    }
  }

  public void Hit()
  {
    Destroy(gameObject);
  }
}
