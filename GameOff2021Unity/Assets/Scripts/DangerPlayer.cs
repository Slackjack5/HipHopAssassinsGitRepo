using UnityEngine;
using UnityEngine.Events;

public class DangerPlayer : MonoBehaviour
{
  [SerializeField] private float duration;
  [SerializeField] private int frequency;
  [SerializeField] private RectTransform topStart;
  [SerializeField] private RectTransform topEnd;
  [SerializeField] private RectTransform bottomStart;
  [SerializeField] private RectTransform bottomEnd;
  [SerializeField] private DangerText dangerPrefab;

  public static readonly UnityEvent onComplete = new UnityEvent();

  private static RectTransform rectTransform;
  private static float _duration;
  private static float remainingTime;
  private static bool isStarted;
  private static float remainingSpawnTime;

  private void Awake()
  {
    rectTransform = GetComponent<RectTransform>();
    _duration = duration;
  }

  private void FixedUpdate()
  {
    if (!isStarted) return;

    remainingSpawnTime -= Time.fixedDeltaTime;
    if (remainingSpawnTime <= 0)
    {
      DangerText topDanger = Instantiate(dangerPrefab, topStart.position, Quaternion.identity, rectTransform);
      topDanger.destination = topEnd;

      DangerText bottomDanger =
        Instantiate(dangerPrefab, bottomStart.position, Quaternion.identity, rectTransform);
      bottomDanger.destination = bottomEnd;

      remainingSpawnTime = duration / frequency;
    }

    remainingTime -= Time.fixedDeltaTime;
    if (remainingTime <= 0)
    {
      isStarted = false;
      onComplete.Invoke();
      onComplete.RemoveAllListeners();
    }
  }

  public static void Play()
  {
    isStarted = true;
    remainingTime = _duration;
    AudioEvents.SetSwitchBoss();
  }
}
