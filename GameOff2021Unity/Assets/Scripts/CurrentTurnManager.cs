using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTurnManager : MonoBehaviour
{
  [SerializeField] private Hero heroOne;
  [SerializeField] private Hero heroTwo;
  [SerializeField] private Hero heroThree;
  [SerializeField] private CombatManager combatManager;

  private CombatManager.CombatState lastState;
  private TextMeshProUGUI nameComponent;
  private VerticalLayoutGroup panel;

  private void Start()
  {
    panel = GetComponentInChildren<VerticalLayoutGroup>();
    nameComponent = GetComponentInChildren<TextMeshProUGUI>();

    lastState = CombatManager.CombatState.UNSPECIFIED;
  }

  private void Update()
  {
    switch (combatManager.CurrentState)
    {
      case CombatManager.CombatState.HERO_ONE:
        panel.gameObject.SetActive(true);
        nameComponent.text = heroOne.combatantName;

        if (lastState != combatManager.CurrentState)
        {
          MoveIndicators();
        }
        
        lastState = CombatManager.CombatState.HERO_ONE;
        break;
      case CombatManager.CombatState.HERO_TWO:
        panel.gameObject.SetActive(true);
        nameComponent.text = heroTwo.combatantName;

        if (lastState != combatManager.CurrentState)
        {
          MoveIndicators();
        }

        lastState = CombatManager.CombatState.HERO_TWO;
        break;
      case CombatManager.CombatState.HERO_THREE:
        panel.gameObject.SetActive(true);
        nameComponent.text = heroThree.combatantName;

        if (lastState != combatManager.CurrentState)
        {
          MoveIndicators();
        }
        
        lastState = CombatManager.CombatState.HERO_THREE;
        break;
      default:
        panel.gameObject.SetActive(false);
        break;
    }
  }

  private void MoveIndicators()
  {
    
  }
}
