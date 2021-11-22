using System;
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
  [SerializeField] private Image fill;
  [SerializeField] private Sprite heroOneFill;
  [SerializeField] private Sprite heroTwoFill;
  [SerializeField] private Sprite heroThreeFill;
  [SerializeField] private GameObject rhythmFill;
  [SerializeField] private GameObject backCommand;

  private RectTransform background;
  private bool isSelectingTarget;
  private Command pendingCommand;

  private void Start()
  {
    Assert.IsTrue(combatManager, "combatManager is empty");

    // Background should be the first child of this object.
    background = GetComponentsInChildren<RectTransform>()[1];
    background.gameObject.SetActive(false);

    RegisterSubmitTargetControls();

    CombatManager.onStateChange.AddListener(OnStateChange);

    HideAllSelectables();
  }

  private void OnStateChange(CombatManager.CombatState state)
  {
    switch (state)
    {
      case CombatManager.CombatState.HeroOne:
        fill.gameObject.SetActive(true);
        rhythmFill.SetActive(false);

        fill.sprite = heroOneFill;
        background.gameObject.SetActive(true);
        OpenTopMenu();
        break;
      case CombatManager.CombatState.HeroTwo:
        fill.gameObject.SetActive(true);
        rhythmFill.SetActive(false);

        fill.sprite = heroTwoFill;
        background.gameObject.SetActive(true);
        OpenTopMenu();
        break;
      case CombatManager.CombatState.HeroThree:
        fill.gameObject.SetActive(true);
        rhythmFill.SetActive(false);

        fill.sprite = heroThreeFill;
        background.gameObject.SetActive(true);
        OpenTopMenu();
        break;
      default:
        fill.gameObject.SetActive(false);
        rhythmFill.SetActive(true);
        HideAllSelectables();
        break;
    }
  }

  public void OpenTopMenu()
  {
    topMenu.SetActive(true);
    paginatedMenu.SetActive(false);

    HideTargetSelector();

    SelectFirstCommand(topMenu);
  }

  public void OpenConsumableMenu()
  {
    OpenPaginatedMenu(DataManager.AllConsumables);
  }

  public void OpenMacroMenu()
  {
    OpenPaginatedMenu(DataManager.AllMacros.Select(macro => new Macro
      {
        name = macro.name,
        description = macro.description,
        patternId = macro.patternId,
        id = macro.id,
        power = macro.power,
        cost = macro.cost
      })
      .Cast<Command>().ToArray());
  }

  public void OpenStanceMenu()
  {
    OpenPaginatedMenu(new Command[]
    {
      new Macro
      {
        name = "Defend",
        description = "Raise guard to halve incoming damage.",
        patternId = 4
      },
      new Macro
      {
        name = "Charge",
        description = "Spend turn to lengthen the time.",
        patternId = 4
      }
    });
  }

  public void SubmitAttack()
  {
    pendingCommand = new Attack
    {
      name = "Attack",
      description = "Attack the enemy.",
      patternId = 1
    };
    OpenTargetSelector();
  }

  private void RegisterSubmitTargetControls()
  {
    foreach (Combatant combatant in CombatManager.Combatants)
    {
      combatant.TargetCursor.onClick.AddListener(() => SubmitTarget(combatant));
    }
  }

  private void HideMenu()
  {
    topMenu.SetActive(false);
    paginatedMenu.SetActive(false);
  }

  private void HideAllSelectables()
  {
    HideMenu();
    HideTargetSelector();
  }

  private void HideTargetSelector()
  {
    isSelectingTarget = false;
    foreach (Combatant combatant in CombatManager.Combatants)
    {
      combatant.TargetCursor.interactable = false;
    }

    backCommand.SetActive(false);
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
      OpenTargetSelector();
    });
  }

  private void OpenTargetSelector()
  {
    HideMenu();

    isSelectingTarget = true;
    foreach (Combatant combatant in CombatManager.Combatants)
    {
      combatant.TargetCursor.interactable = true;
    }

    backCommand.SetActive(true);

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
