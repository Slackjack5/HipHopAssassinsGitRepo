using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CurrentTurnManager : MonoBehaviour
{
  private Hero[] heroes;
  private CombatManager.CombatState lastState;
  private TextMeshProUGUI nameComponent;
  private VerticalLayoutGroup panel;

  private void Start()
  {
    panel = GetComponentInChildren<VerticalLayoutGroup>();
    nameComponent = GetComponentInChildren<TextMeshProUGUI>();

    panel.gameObject.SetActive(false);

    heroes = CombatManager.Heroes;
    lastState = CombatManager.CombatState.Unspecified;
    CombatManager.onChangeState.AddListener(ReadState);
  }

  private void ReadState(CombatManager.CombatState state)
  {
    switch (state)
    {
      case CombatManager.CombatState.Unspecified:
      case CombatManager.CombatState.PreStart:
      case CombatManager.CombatState.Start:
        break;
      case CombatManager.CombatState.HeroOne:
        panel.gameObject.SetActive(true);
        nameComponent.text = heroes[0].Name;

        if (lastState != CombatManager.CurrentState)
        {
          MoveIndicators();
          SpotlightHero();
        }

        break;
      case CombatManager.CombatState.HeroTwo:
        panel.gameObject.SetActive(true);
        nameComponent.text = heroes[1].Name;

        if (lastState != CombatManager.CurrentState)
        {
          MoveIndicators();
          SpotlightHero();
        }

        break;
      case CombatManager.CombatState.HeroThree:
        panel.gameObject.SetActive(true);
        nameComponent.text = heroes[2].Name;

        if (lastState != CombatManager.CurrentState)
        {
          MoveIndicators();
          SpotlightHero();
        }

        break;
      default:
        panel.gameObject.SetActive(false);
        ResetAllHeroPositions();
        break;
    }

    lastState = state;
  }

  private void MoveIndicators()
  {
  }

  private void ResetAllHeroPositions()
  {
    foreach (Hero hero in heroes)
    {
      hero.ResetPosition();
    }
  }

  private void SpotlightHero()
  {
    switch (CombatManager.CurrentState)
    {
      case CombatManager.CombatState.HeroOne:
        heroes[0].Spotlight();
        heroes[1].ResetPosition();
        heroes[2].ResetPosition();
        break;
      case CombatManager.CombatState.HeroTwo:
        heroes[1].Spotlight();
        heroes[0].ResetPosition();
        heroes[2].ResetPosition();
        break;
      case CombatManager.CombatState.HeroThree:
        heroes[2].Spotlight();
        heroes[0].ResetPosition();
        heroes[1].ResetPosition();
        break;
      default:
        ResetAllHeroPositions();
        break;
    }
  }
}
