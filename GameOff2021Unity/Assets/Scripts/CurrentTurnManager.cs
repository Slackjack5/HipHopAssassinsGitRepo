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
  [SerializeField] private float spotlightDistance;
  [SerializeField] private CombatManager combatManager;

  private float initialPositionX;
  private CombatManager.CombatState lastState;
  private TextMeshProUGUI nameComponent;
  private VerticalLayoutGroup panel;

  private void Start()
  {
    panel = GetComponentInChildren<VerticalLayoutGroup>();
    nameComponent = GetComponentInChildren<TextMeshProUGUI>();

    initialPositionX = heroOne.transform.position.x;
    lastState = CombatManager.CombatState.UNSPECIFIED;
  }

  private void Update()
  {
    switch (combatManager.CurrentState)
    {
      case CombatManager.CombatState.HERO_ONE:
        panel.gameObject.SetActive(true);
        nameComponent.text = heroOne.Name;

        if (lastState != combatManager.CurrentState)
        {
          MoveIndicators();
          SpotlightHero();
        }
        
        break;
      case CombatManager.CombatState.HERO_TWO:
        panel.gameObject.SetActive(true);
        nameComponent.text = heroTwo.Name;

        if (lastState != combatManager.CurrentState)
        {
          MoveIndicators();
          SpotlightHero();
        }

        break;
      case CombatManager.CombatState.HERO_THREE:
        panel.gameObject.SetActive(true);
        nameComponent.text = heroThree.Name;

        if (lastState != combatManager.CurrentState)
        {
          MoveIndicators();
          SpotlightHero();
        }
        
        break;
      default:
        panel.gameObject.SetActive(false);
        ResetPosition(heroOne);
        ResetPosition(heroTwo);
        ResetPosition(heroThree);
        break;
    }

    lastState = combatManager.CurrentState;
  }

  private void MoveIndicators()
  {
    
  }

  private void ResetPosition(Hero hero)
  {
    hero.transform.position = new Vector2(initialPositionX, hero.transform.position.y);
  }

  private void SpotlightHero()
  {
    switch (combatManager.CurrentState)
    {
      case CombatManager.CombatState.HERO_ONE:
        heroOne.transform.Translate(new Vector2(spotlightDistance, 0));
        ResetPosition(heroTwo);
        ResetPosition(heroThree);
        break;
      case CombatManager.CombatState.HERO_TWO:
        heroTwo.transform.Translate(new Vector2(spotlightDistance, 0));
        ResetPosition(heroOne);
        ResetPosition(heroThree);
        break;
      case CombatManager.CombatState.HERO_THREE:
        heroThree.transform.Translate(new Vector2(spotlightDistance, 0));
        ResetPosition(heroOne);
        ResetPosition(heroTwo);
        break;
      default:
        ResetPosition(heroOne);
        ResetPosition(heroTwo);
        ResetPosition(heroThree);
        break;
    }
  }
}
