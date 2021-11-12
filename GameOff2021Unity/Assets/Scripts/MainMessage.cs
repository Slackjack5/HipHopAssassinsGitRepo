using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMessage : MonoBehaviour
{
  [SerializeField] private CombatManager combatManager;

  private TextMeshProUGUI message;

  private void Start()
  {
    message = GetComponent<TextMeshProUGUI>();
  }

  private void Update()
  {
    switch (combatManager.CurrentState)
    {
      case CombatManager.CombatState.START:
        message.enabled = true;
        message.text = "FIGHT";
        break;
      case CombatManager.CombatState.LOSE:
        message.enabled = true;
        message.text = "GAME OVER";
        break;
      default:
        message.enabled = false;
        break;
    }
  }
}
