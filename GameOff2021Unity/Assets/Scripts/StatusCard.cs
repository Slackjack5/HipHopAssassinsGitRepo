using TMPro;
using UnityEngine;

public class StatusCard : MonoBehaviour
{
  [SerializeField] private Hero hero;
  [SerializeField] private GameObject activePanel;
  [SerializeField] private GameObject inactivePanel;

  private TextMeshProUGUI activeHeroName;
  private TextMeshProUGUI inactiveHeroName;
  private StatusBar activeHealthBar;
  private StatusBar activeStaminaBar;
  private StatusBar inactiveHealthBar;
  private StatusBar inactiveStaminaBar;
  private bool isActive;

  private void Start()
  {
    // Name should be the first TextMeshProUGUI component encountered.
    activeHeroName = activePanel.GetComponentInChildren<TextMeshProUGUI>();
    activeHeroName.text = hero.Name;

    inactiveHeroName = inactivePanel.GetComponentInChildren<TextMeshProUGUI>();
    inactiveHeroName.text = hero.Name;

    activeHealthBar = activePanel.GetComponentsInChildren<StatusBar>()[0];
    activeHealthBar.SetMaxValue(hero.MaxHealth);
    activeHealthBar.SetValue(hero.CurrentHealth);

    activeStaminaBar = activePanel.GetComponentsInChildren<StatusBar>()[1];
    activeStaminaBar.SetMaxValue(hero.MaxStamina);
    activeStaminaBar.SetValue(hero.CurrentStamina);

    inactiveHealthBar = inactivePanel.GetComponentsInChildren<StatusBar>()[0];
    inactiveHealthBar.SetMaxValue(hero.MaxHealth);
    inactiveHealthBar.SetValue(hero.CurrentHealth);

    inactiveStaminaBar = inactivePanel.GetComponentsInChildren<StatusBar>()[1];
    inactiveStaminaBar.SetMaxValue(hero.MaxStamina);
    inactiveStaminaBar.SetValue(hero.CurrentStamina);
  }

  private void Update()
  {
    activeHealthBar.SetValue(hero.CurrentHealth);
    activeStaminaBar.SetValue(hero.CurrentStamina);
    inactiveHealthBar.SetValue(hero.CurrentHealth);
    inactiveStaminaBar.SetValue(hero.CurrentStamina);

    switch (CombatManager.CurrentState)
    {
      case CombatManager.CombatState.HeroOne:
        isActive = hero.HeroId == 1;
        break;
      case CombatManager.CombatState.HeroTwo:
        isActive = hero.HeroId == 2;
        break;
      case CombatManager.CombatState.HeroThree:
        isActive = hero.HeroId == 3;
        break;
      default:
        isActive = false;
        break;
    }

    activePanel.SetActive(isActive);
    inactivePanel.SetActive(!isActive);
  }
}
