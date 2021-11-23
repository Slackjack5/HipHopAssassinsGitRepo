using TMPro;
using UnityEngine;

public class MainMessage : MonoBehaviour
{
  private TextMeshProUGUI message;

  private void Start()
  {
    message = GetComponent<TextMeshProUGUI>();
  }

  private void Update()
  {
    switch (CombatManager.CurrentState)
    {
      case CombatManager.State.Start:
        Display("FIGHT");
        break;
      case CombatManager.State.Lose:
        Display("GAME OVER");
        break;
      case CombatManager.State.Win:
        Display("VICTORY");
        break;
      default:
        message.enabled = false;
        break;
    }
  }

  private void Display(string text)
  {
    message.enabled = true;
    message.text = text;
  }
}
