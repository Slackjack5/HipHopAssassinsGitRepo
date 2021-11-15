using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
  [SerializeField] private GameObject topMenu;
  [SerializeField] private GameObject paginatedMenu;
  [SerializeField] private CombatManager combatManager;

  private CombatManager.CombatState lastState;

  private void Start()
  {
    Assert.IsTrue(combatManager, "combatManager is empty");

    lastState = CombatManager.CombatState.Unspecified;
  }

  private void Update()
  {
    switch (CombatManager.CurrentState)
    {
      case CombatManager.CombatState.HeroOne:
      case CombatManager.CombatState.HeroTwo:
      case CombatManager.CombatState.HeroThree:
        if (lastState != CombatManager.CurrentState)
        {
          OpenTopMenu();
        }

        break;
      default:
        HideMenu();
        break;
    }

    lastState = CombatManager.CurrentState;
  }

  public void OpenTopMenu()
  {
    topMenu.SetActive(true);
    paginatedMenu.SetActive(false);

    SelectFirstCommand(topMenu);
  }

  public void OpenConsumableMenu()
  {
    // Convert list of Consumables to a list of Commands.
    OpenPaginatedMenu(DataManager.AllConsumables.Select(
        consumable => new Command(consumable.name, consumable.description, Command.Type.Consumable, 2))
      .ToArray()
    );
  }

  public void OpenMacroMenu()
  {
    // Convert list of Macros to a list of Commands.
    OpenPaginatedMenu(DataManager.AllMacros.Select(
        macro => new Command(macro.name, macro.description, Command.Type.Macro, 3)).ToArray()
    );
  }

  public void OpenStanceMenu()
  {
    OpenPaginatedMenu(new[]
    {
      new Command("Defend", "Raise guard to halve incoming damage.", Command.Type.Defend, 4),
      new Command("Charge", "Spend turn to lengthen the time.", Command.Type.Charge, 4)
    });
  }

  public void SubmitAttack()
  {
    combatManager.SubmitCommand(new Command("Attack", "Attack the enemy.", Command.Type.Attack, 1));
  }

  private void HideMenu()
  {
    topMenu.SetActive(false);
    paginatedMenu.SetActive(false);
  }

  private void OpenPaginatedMenu(Command[] commands)
  {
    topMenu.SetActive(false);
    paginatedMenu.SetActive(true);

    paginatedMenu.GetComponent<CommandLoader>().LoadCommands(commands);
  }

  private static void SelectFirstCommand(GameObject menu)
  {
    EventSystem.current.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);
  }
}
