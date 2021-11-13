using System.Linq;
using UnityEngine;
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
    lastState = CombatManager.CombatState.Unspecified;
  }

  private void Update()
  {
    switch (combatManager.CurrentState)
    {
      case CombatManager.CombatState.HeroOne:
      case CombatManager.CombatState.HeroTwo:
      case CombatManager.CombatState.HeroThree:
        if (lastState != combatManager.CurrentState)
        {
          OpenTopMenu();
        }
        break;
      default:
        HideMenu();
        break;
    }

    lastState = combatManager.CurrentState;
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
    OpenPaginatedMenu(DataManager.AllConsumables.Select(consumable => new Command(consumable.name, consumable.description, 2)).ToArray());
  }

  public void OpenSpellMenu()
  {
    // Convert list of Spells to a list of Commands.
    OpenPaginatedMenu(DataManager.AllSpells.Select(spell => new Command(spell.name, spell.description, 3)).ToArray());
  }

  public void OpenStanceMenu()
  {
    OpenPaginatedMenu(new Command[]
    {
      new Command("Defend", "Raise guard to halve incoming damage.", 4),
      new Command("Charge", "Spend turn to lengthen the time.", 4)
    });
  }

  public void SubmitAttack()
  {
    combatManager.SubmitCommand(new Command("Attack", "Attack the enemy.", 1));
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
