using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
  [SerializeField] private GameObject topMenu;
  [SerializeField] private GameObject paginatedMenu;

  private void Awake()
  {
    OpenTopMenu();
  }

  public void OpenTopMenu()
  {
    topMenu.SetActive(true);
    paginatedMenu.SetActive(false);

    SelectFirstCommand(topMenu);
  }

  public void OpenAttackMenu()
  {
    OpenPaginatedMenu(new Command[]
    {
      new Command("Attack 1", "This is attack 1."),
      new Command("Attack 2", "This is attack 2."),
      new Command("Attack 3", "This is attack 3."),
      new Command("Attack 4", "This is attack 4.")
    });
  }

  public void OpenConsumableMenu()
  {
    // Convert list of Consumables to a list of Commands.
    List<Command> commands = new List<Command>();
    foreach (Consumable consumable in DataManager.AllConsumables)
    {
      commands.Add(new Command(consumable.name, consumable.description));
    }
    OpenPaginatedMenu(commands.ToArray());
  }

  public void OpenSpellMenu()
  {
    // Convert list of Spells to a list of Commands.
    List<Command> commands = new List<Command>();
    foreach (Spell spell in DataManager.AllSpells)
    {
      commands.Add(new Command(spell.name, spell.description));
    }
    OpenPaginatedMenu(commands.ToArray());
  }

  public void OpenStanceMenu()
  {
    OpenPaginatedMenu(new Command[]
    {
      new Command("Defend", "Raise guard to halve incoming damage."),
      new Command("Charge", "Spend turn to lengthen the time.")
    });
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
