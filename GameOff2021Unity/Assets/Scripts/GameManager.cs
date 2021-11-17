using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
  [SerializeField] private GameObject pauseMenuCanvas;

  private static GameObject pauseMenu;
  private static bool isPaused;

  private void Awake()
  {
    pauseMenu = pauseMenuCanvas;
  }

  private void Update()
  {
    if (Keyboard.current.escapeKey.wasPressedThisFrame)
    {
      OnPause();
    }
  }

  private static void OnPause()
  {
    if (isPaused)
    {
      Time.timeScale = 1;
      pauseMenu.SetActive(false);
      isPaused = false;
    }
    else
    {
      Time.timeScale = 0;
      pauseMenu.SetActive(true);
      isPaused = true;
    }
  }
}
