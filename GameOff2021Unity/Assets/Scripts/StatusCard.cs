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
  private TurnIndicator turnIndicator;
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

    turnIndicator = GetComponentInChildren<TurnIndicator>();

    CombatManager.onStateChange.AddListener(OnStateChange);

    isActive = false;
    activePanel.SetActive(false);
    inactivePanel.SetActive(true);
  }

  private void OnStateChange(CombatManager.CombatState state)
  {
    switch (state)
    {
      case CombatManager.CombatState.Unspecified:
      case CombatManager.CombatState.PreStart:
      case CombatManager.CombatState.Start:
        break;
      case CombatManager.CombatState.HeroOne:
        isActive = hero.HeroId == 1;
        if (isActive)
        {
          turnIndicator.Show();
        }
        else
        {
          turnIndicator.Hide();
        }

        break;
      case CombatManager.CombatState.HeroTwo:
        isActive = hero.HeroId == 2;
        if (isActive)
        {
          turnIndicator.Show();
        }
        else
        {
          turnIndicator.Hide();
        }

        break;
      case CombatManager.CombatState.HeroThree:
        isActive = hero.HeroId == 3;
        if (isActive)
        {
          turnIndicator.Show();
        }
        else
        {
          turnIndicator.Hide();
        }

        break;
      default:
        isActive = false;
        turnIndicator.Hide();

        break;
    }

    activePanel.SetActive(isActive);
    inactivePanel.SetActive(!isActive);
  }

  private void Update()
  {
    activeHealthBar.SetValue(hero.CurrentHealth);
    activeStaminaBar.SetValue(hero.CurrentStamina);
    inactiveHealthBar.SetValue(hero.CurrentHealth);
    inactiveStaminaBar.SetValue(hero.CurrentStamina);
  }
}
