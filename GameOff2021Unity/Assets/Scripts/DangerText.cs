using UnityEngine;

public class DangerText : MonoBehaviour
{
  [SerializeField] private float duration;

  public RectTransform destination;

  private Vector2 startPosition;
  private float timeElapsed;

  private void Start()
  {
    startPosition = transform.position;
  }

  private void Update()
  {
    if (timeElapsed < duration)
    {
      transform.position =
        Vector2.Lerp(startPosition, destination.position, timeElapsed / duration);
      timeElapsed += Time.deltaTime;
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
