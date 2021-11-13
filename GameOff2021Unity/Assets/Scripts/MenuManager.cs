using System.Collections;
using System.Collections.Generic;
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
    lastState = CombatManager.CombatState.UNSPECIFIED;
  }

  private void Update()
  {
    switch (combatManager.CurrentState)
    {
      case CombatManager.CombatState.HERO_ONE:
      case CombatManager.CombatState.HERO_TWO:
      case CombatManager.CombatState.HERO_THREE:
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

  public void HideMenu()
  {
    topMenu.SetActive(false);
    paginatedMenu.SetActive(false);
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
    List<Command> commands = new List<Command>();
    foreach (Consumable consumable in DataManager.AllConsumables)
    {
      commands.Add(new Command(consumable.name, consumable.description, 2));
    }
    OpenPaginatedMenu(commands.ToArray());
  }

  public void OpenSpellMenu()
  {
    // Convert list of Spells to a list of Commands.
    List<Command> commands = new List<Command>();
    foreach (Spell spell in DataManager.AllSpells)
    {
      commands.Add(new Command(spell.name, spell.description, 3));
    }
    OpenPaginatedMenu(commands.ToArray());
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

  private void OpenPaginatedMenu(Command[] commands)
  {
    topMenu.SetActive(false);
    paginatedMenu.SetActive(true);

    paginatedMenu.GetComponent<CommandLoader>().LoadCommands(commands);
  }

  private void SelectFirstCommand(GameObject menu)
  {
    EventSystem.current.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);
  }
}
