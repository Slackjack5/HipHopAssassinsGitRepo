using UnityEngine;

public class BeatCircle : MonoBehaviour
{
  [HideInInspector] public Transform spawnerPos;
  [HideInInspector] public Transform centerPos;
  [HideInInspector] public Transform endPos;
  [HideInInspector] public float travelTime;

  private float currentTime;
  private bool reachedMiddle;

  private void Update()
  {
    if (currentTime < travelTime)
    {
      transform.position = reachedMiddle
        ? Vector2.Lerp(centerPos.position, endPos.position, currentTime / travelTime)
        : Vector2.Lerp(spawnerPos.position, centerPos.position, currentTime / travelTime);
      currentTime += Time.deltaTime;
    }
    else
    {
      if (reachedMiddle)
      {
        // Reached the end of the track, so destroy this object.
        Destroy(gameObject);
      }
      else
      {
        reachedMiddle = true;
        currentTime = 0;
      }
    }
  }

  public void Hit()
  {
    //Destroy(gameObject);
    gameObject.GetComponent<Animator>().SetBool("Hit", true);
  }

  public void Destroy()
  {
    Destroy(gameObject);
  }
}
