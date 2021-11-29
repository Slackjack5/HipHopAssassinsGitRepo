using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
  [SerializeField] private GameObject descriptionPanel;

  private RectTransform background;
  private bool isSelectingTarget;
  private Command pendingCommand;
  private CommandLoader commandLoader;

  private void Start()
  {
    Assert.IsTrue(combatManager, "combatManager is empty");

    // Background should be the first child of this object.
    background = GetComponentsInChildren<RectTransform>()[1];
    background.gameObject.SetActive(false);

    HideMenu();
    backCommand.SetActive(false);
    descriptionPanel.SetActive(false);

    RegisterSubmitTargetControls(CombatManager.Heroes);

    CombatManager.onStateChange.AddListener(OnCombatStateChange);

    commandLoader = paginatedMenu.GetComponent<CommandLoader>();
    commandLoader.onSubmitCommand.AddListener(command =>
    {
      pendingCommand = command;
      OpenTargetSelector();
    });
  }

  private void Update()
  {
    if (paginatedMenu.activeSelf)
    {
      descriptionPanel.SetActive(true);

      var commandRef = EventSystem.current.currentSelectedGameObject.GetComponent<CommandRef>();
      if (commandRef == null)
      {
        descriptionPanel.SetActive(false);
      }
      else
      {
        descriptionPanel.GetComponentInChildren<TextMeshProUGUI>().text = commandRef.command.description;
      }
    }
    else
    {
      descriptionPanel.SetActive(false);
    }
  }

  private void OnCombatStateChange(CombatManager.State state)
  {
    switch (state)
    {
      case CombatManager.State.PreStart:
        RegisterSubmitTargetControls(CombatManager.Monsters);
        HideAllSelectables();
        break;
      case CombatManager.State.HeroOne:
        UpdateCommandMenu(heroOneFill);
        break;
      case CombatManager.State.HeroTwo:
        UpdateCommandMenu(heroTwoFill);
        break;
      case CombatManager.State.HeroThree:
        UpdateCommandMenu(heroThreeFill);
        break;
      case CombatManager.State.Lose:
      case CombatManager.State.Win:
        background.gameObject.SetActive(false);
        HideAllSelectables();
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


    //Sound Effect
    AkSoundEngine.PostEvent("Play_UISelect", gameObject);
  }

  public void OpenConsumableMenu()
  {
    OpenPaginatedMenu(EncounterManager.ConsumablesOwned.ToArray());


    //Sound Effect
    AkSoundEngine.PostEvent("Play_UISelect", gameObject);
  }

  public void OpenMacroMenu()
  {
    int[] macroIds = CombatManager.CurrentHero.MacroIds;

    List<Command> macros = macroIds.Select(macroId => DataManager.AllMacros[macroId - 1])
      .Select(macro => new Macro
      {
        name = macro.name,
        description = macro.description,
        patternId = macro.patternId,
        selectMonster = macro.selectMonster,
        needsTarget = macro.needsTarget,
        id = macro.id,
        power = macro.power,
        cost = macro.cost,
        HasEnoughStamina = CombatManager.CurrentHero.CanCastMacro(macro)
      })
      .Cast<Command>()
      .ToList();

    OpenPaginatedMenu(macros.ToArray());


    //Sound Effect
    AkSoundEngine.PostEvent("Play_UISelect", gameObject);
  }

  public void OpenStanceMenu()
  {
    OpenPaginatedMenu(DataManager.AllStances);
  }

  public void SubmitAttack()
  {
    int patternId = CombatManager.CurrentHero.AttackPatternId;

    pendingCommand = new Attack
    {
      name = "Attack",
      description = "Attack the enemy.",
      patternId = patternId,
      selectMonster = true,
      needsTarget = true
    };

    //Sound Effect
    AkSoundEngine.PostEvent("Play_UISelect", gameObject);

    OpenTargetSelector();
  }

  private void RegisterSubmitTargetControls(IEnumerable<Combatant> combatants)
  {
    foreach (Combatant combatant in combatants)
    {
      combatant.TargetCursor.onClick.AddListener(() => SubmitTarget(combatant));
    }
  }

  private void UpdateCommandMenu(Sprite heroFill)
  {
    background.gameObject.SetActive(true);
    fill.gameObject.SetActive(true);
    rhythmFill.SetActive(false);

    fill.sprite = heroFill;

    // Fill for the description panel should be the second image.
    descriptionPanel.GetComponentsInChildren<Image>()[1].sprite = heroFill;

    OpenTopMenu();
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

    commandLoader.Load(commands);
  }

  private void OpenTargetSelector()
  {
    HideMenu();

    isSelectingTarget = true;
    foreach (Combatant combatant in CombatManager.Combatants.Where(combatant => !(combatant is Monster {IsDead: true})))
    {
      combatant.TargetCursor.interactable = true;
    }

    backCommand.SetActive(true);

    if (pendingCommand.selectMonster)
    {
      SelectFirstMonster();
    }
    else
    {
      SelectFirstHero();
    }
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

    //Sound Effect
    AkSoundEngine.PostEvent("Play_UISelect", gameObject);
  }

  private void SelectFirstCommand(GameObject menu)
  {
    if (isSelectingTarget)
    {
      Debug.LogWarning("Failed selecting first command. We are selecting a target!");
      return;
    }

    EventSystem.current.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);

    //Sound Effect
    AkSoundEngine.PostEvent("Play_UIMove", gameObject);
  }

  private void SelectFirstHero()
  {
    if (!isSelectingTarget)
    {
      Debug.LogWarning("Failed selecting first hero. We are still in the command menu!");
      return;
    }

    EventSystem.current.SetSelectedGameObject(CombatManager.Heroes[0].TargetCursor.gameObject);
  }

  private void SelectFirstMonster()
  {
    if (!isSelectingTarget)
    {
      Debug.LogWarning("Failed selecting first monster. We are still in the command menu!");
      return;
    }

    EventSystem.current.SetSelectedGameObject(CombatManager.FirstLivingMonster.TargetCursor.gameObject);
  }
}
