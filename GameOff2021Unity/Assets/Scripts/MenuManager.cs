using System.Collections.Generic;
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
  private readonly List<Button> targetCursors = new List<Button>();
  private bool isSelectingTarget;
  private Command pendingCommand;

  private void Start()
  {
    Assert.IsTrue(combatManager, "combatManager is empty");

    lastState = CombatManager.CombatState.Unspecified;

    RegisterSubmitTargetControls();
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
        HideTargetMenu();
        break;
    }

    lastState = CombatManager.CurrentState;
  }

  public void OpenTopMenu()
  {
    topMenu.SetActive(true);
    paginatedMenu.SetActive(false);

    HideTargetMenu();

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
    pendingCommand = new Command("Attack", "Attack the enemy.", Command.Type.Attack, 1);
    OpenTargetMenu();
  }

  private void RegisterSubmitTargetControls()
  {
    foreach (Combatant combatant in CombatManager.Combatants)
    {
      combatant.TargetCursor.onClick.AddListener(() => SubmitTarget(combatant));
      targetCursors.Add(combatant.TargetCursor);
    }
  }

  private void HideMenu()
  {
    topMenu.SetActive(false);
    paginatedMenu.SetActive(false);
  }

  private void HideTargetMenu()
  {
    isSelectingTarget = false;
    foreach (Button targetCursor in targetCursors)
    {
      targetCursor.interactable = false;
    }
  }

  private void OpenPaginatedMenu(Command[] commands)
  {
    topMenu.SetActive(false);
    paginatedMenu.SetActive(true);

    var commandLoader = paginatedMenu.GetComponent<CommandLoader>();
    commandLoader.LoadCommands(commands);
    commandLoader.onSubmitCommand.AddListener(command =>
    {
      pendingCommand = command;
      OpenTargetMenu();
    });
  }

  private void OpenTargetMenu()
  {
    HideMenu();

    isSelectingTarget = true;
    foreach (Button targetCursor in targetCursors)
    {
      targetCursor.interactable = true;
    }

    SelectFirstTarget();
  }

  private void SubmitTarget(Combatant combatant)
  {
    if (pendingCommand == null)
    {
      Debug.LogError("Failed to submit target. Pending command does not exist!");
      return;
    }

    pendingCommand.SetTarget(combatant);
    combatManager.SubmitCommand(pendingCommand);
  }

  private void SelectFirstCommand(GameObject menu)
  {
    if (isSelectingTarget)
    {
      Debug.LogWarning("Failed selecting first command. We are selecting a target!");
      return;
    }

    EventSystem.current.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);
  }

  private void SelectFirstTarget()
  {
    if (!isSelectingTarget)
    {
      Debug.LogWarning("Failed selecting first target. We are still in the command menu!");
      return;
    }

    EventSystem.current.SetSelectedGameObject(CombatManager.Heroes[0].TargetCursor.gameObject);
  }
}
