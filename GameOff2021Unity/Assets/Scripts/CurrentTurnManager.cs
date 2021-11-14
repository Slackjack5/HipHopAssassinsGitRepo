using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CurrentTurnManager : MonoBehaviour
{
  [SerializeField] private float spotlightDistance;
  [SerializeField] private CombatManager combatManager;

  private Hero[] heroes;
  private float initialPositionX;
  private CombatManager.CombatState lastState;
  private TextMeshProUGUI nameComponent;
  private VerticalLayoutGroup panel;

  private void Start()
  {
    Assert.IsTrue(combatManager, "combatManager is empty");

    panel = GetComponentInChildren<VerticalLayoutGroup>();
    nameComponent = GetComponentInChildren<TextMeshProUGUI>();

    heroes = combatManager.Heroes;
    initialPositionX = heroes[0].transform.position.x;
    lastState = CombatManager.CombatState.Unspecified;
  }

  private void Update()
  {
    switch (CombatManager.CurrentState)
    {
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
        ResetAllPositions();
        break;
    }

    lastState = CombatManager.CurrentState;
  }

  private void MoveIndicators()
  {
  }

  private void ResetAllPositions()
  {
    ResetPosition(heroes[0]);
    ResetPosition(heroes[1]);
    ResetPosition(heroes[2]);
  }

  private void ResetPosition(Hero hero)
  {
    Transform heroTransform = hero.transform;
    heroTransform.position = new Vector2(initialPositionX, heroTransform.position.y);
  }

  private void SpotlightHero()
  {
    switch (CombatManager.CurrentState)
    {
      case CombatManager.CombatState.HeroOne:
        heroes[0].transform.Translate(new Vector2(spotlightDistance, 0));
        ResetPosition(heroes[1]);
        ResetPosition(heroes[2]);
        break;
      case CombatManager.CombatState.HeroTwo:
        heroes[1].transform.Translate(new Vector2(spotlightDistance, 0));
        ResetPosition(heroes[0]);
        ResetPosition(heroes[2]);
        break;
      case CombatManager.CombatState.HeroThree:
        heroes[2].transform.Translate(new Vector2(spotlightDistance, 0));
        ResetPosition(heroes[0]);
        ResetPosition(heroes[1]);
        break;
      default:
        ResetAllPositions();
        break;
    }
  }
}
