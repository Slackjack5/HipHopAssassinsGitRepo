using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
  [SerializeField] private float timeLimit = 120f; // In seconds

  public static readonly UnityEvent onExpire = new UnityEvent();

  private enum State
  {
    Inactive,
    Running,
    Paused,
    Expired
  }

  private static ProgressBar progressBar;
  private static float _timeLimit;
  private static float currentTime;
  private static State currentState;
  private static float pauseTime;

  private void Start()
  {
    progressBar = GetComponentInChildren<ProgressBar>();
    progressBar.SetMaxValue(timeLimit);

    _timeLimit = timeLimit;

    currentState = State.Inactive;
  }

  private void Update()
  {
    progressBar.SetValue(currentTime);

    switch (currentState)
    {
      case State.Inactive:
        progressBar.gameObject.SetActive(false);
        break;
      case State.Paused:
        pauseTime -= Time.deltaTime;
        if (pauseTime <= 0)
        {
          currentState = State.Running;
        }

        break;
      case State.Running:
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
          currentTime = 0;
          onExpire.Invoke();
          currentState = State.Expired;
        }

        break;
    }
  }

  public static void Activate()
  {
    if (currentState == State.Inactive)
    {
      progressBar.gameObject.SetActive(true);
      currentTime = _timeLimit;
      currentState = State.Running;
    }
    else
    {
      Debug.LogWarning("Failed to activate timer. Timer is currently running or has not been reset.");
    }
  }

  public static void ResetState()
  {
    currentState = State.Inactive;
  }

  public static void Pause(int time)
  {
    pauseTime = time;
    currentState = State.Paused;
  }
}
