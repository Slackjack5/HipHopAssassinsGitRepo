using UnityEngine;
using UnityEngine.UI;

public class CurrentTurnManager : MonoBehaviour
{
  private Hero[] heroes;
  private CombatManager.CombatState lastState;
  private Image image;

  private void Start()
  {
    image = GetComponentInChildren<Image>();

    image.gameObject.SetActive(false);

    heroes = CombatManager.Heroes;
    lastState = CombatManager.CombatState.Unspecified;
    CombatManager.onChangeState.AddListener(OnChangeState);
  }

  private void OnChangeState(CombatManager.CombatState state)
  {
    switch (state)
    {
      case CombatManager.CombatState.Unspecified:
      case CombatManager.CombatState.PreStart:
      case CombatManager.CombatState.Start:
        break;
      case CombatManager.CombatState.HeroOne:
        image.gameObject.SetActive(true);

        if (lastState != CombatManager.CurrentState)
        {
          MoveIndicators();
          SpotlightHero();
        }

        break;
      case CombatManager.CombatState.HeroTwo:
        image.gameObject.SetActive(true);

        if (lastState != CombatManager.CurrentState)
        {
          MoveIndicators();
          SpotlightHero();
        }

        break;
      case CombatManager.CombatState.HeroThree:
        image.gameObject.SetActive(true);

        if (lastState != CombatManager.CurrentState)
        {
          MoveIndicators();
          SpotlightHero();
        }

        break;
      default:
        image.gameObject.SetActive(false);
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
