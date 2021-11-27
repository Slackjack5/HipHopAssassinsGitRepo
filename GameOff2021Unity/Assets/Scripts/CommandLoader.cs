using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class CommandLoader : MonoBehaviour
{
  [SerializeField] private GameObject commandPrefab;
  [SerializeField] private GameObject commandPanel;
  [SerializeField] private TextMeshProUGUI pageLabel;
  [SerializeField] private GameObject backCommand;
  [SerializeField] private Color priceColor;
  [SerializeField] private Color staminaColor;
  [SerializeField] private int pageSize = 6;

  public readonly UnityEvent<Command> onSubmitCommand = new UnityEvent<Command>();

  private Command[] commands;
  private int currentPage = 1;
  private int totalPages;
  private bool _isShop;

  public void Load(Command[] commandsToLoad, bool isShop = false)
  {
    _isShop = isShop;
    commands = commandsToLoad;

    currentPage = 1;
    totalPages = commandsToLoad.Length / pageSize;
    if (commandsToLoad.Length % pageSize > 0)
    {
      totalPages++;
    }

    DisplayCommands();
  }

  private void ClearDisplay()
  {
    foreach (Transform child in commandPanel.transform)
    {
      Destroy(child.gameObject);
    }
  }

  private void DisplayCommands()
  {
    ClearDisplay();
    SetPageLabel();

    if (commands.Length == 0)
    {
      SelectCommand(backCommand);
      return;
    }

    GameObject firstCommand = null;

    for (var i = 0; i < pageSize; i++)
    {
      // If currentPage is 0, we want to fetch items 0 to 5.
      // If currentPage is 1, we want to fetch items 6 to 11, and so on.
      int index = i + pageSize * (currentPage - 1);
      if (index >= commands.Length) continue;

      Command command = commands[index];

      // Don't create a command for a consumable that the player does not currently have.
      if (!_isShop && command is Consumable {AmountOwned: 0}) continue;

      GameObject commandObject = Instantiate(commandPrefab, commandPanel.transform);
      if (firstCommand == null)
      {
        firstCommand = commandObject;
      }

      // When i is even, the command appears in the left column.
      // When i is odd, the command appears in the right column.
      if (i % 2 == 0)
      {
        if (currentPage > 1)
        {
          RegisterPageControl(commandObject, Keyboard.current.leftArrowKey, LoadPreviousPage);
        }
      }
      else
      {
        if (currentPage < totalPages)
        {
          RegisterPageControl(commandObject, Keyboard.current.rightArrowKey, LoadNextPage);
        }
      }

      var button = commandObject.GetComponentInChildren<Button>();
      button.onClick.AddListener(() => SubmitCommand(command));
      button.gameObject.AddComponent<CommandRef>().command = command;

      TextMeshProUGUI[] textComponents = commandObject.GetComponentsInChildren<TextMeshProUGUI>();

      TextMeshProUGUI commandName = textComponents[0];
      commandName.text = command.name;

      TextMeshProUGUI number = textComponents[1];
      switch (command)
      {
        case Consumable consumable when _isShop:
          number.text = $"${consumable.cost}";
          number.color = priceColor;
          break;
        case Consumable consumable:
          number.text = $"x{consumable.AmountOwned}";
          number.color = Color.white;
          break;
        case Macro macro:
          number.text = $"{macro.cost}";
          number.color = staminaColor;
          break;
        default:
          number.text = "";
          break;
      }
    }

    SelectCommand(firstCommand);
  }

  private void LoadNextPage()
  {
    currentPage++;
    DisplayCommands();
  }

  private void LoadPreviousPage()
  {
    currentPage--;
    DisplayCommands();
  }

  private void SetPageLabel()
  {
    if (totalPages == 0)
    {
      pageLabel.text = "";
    }
    else
    {
      pageLabel.text = currentPage + " of " + totalPages;
    }
  }

  private void SubmitCommand(Command command)
  {
    onSubmitCommand.Invoke(command);
  }

  private static void RegisterPageControl(GameObject commandObject, KeyControl keyControl, UnityAction action)
  {
    GameObject buttonObject = commandObject.GetComponentInChildren<Button>().gameObject;
    var control = buttonObject.AddComponent<CommandPageControl>();
    control.keyControl = keyControl;
    control.activate.AddListener(action);
  }

  private static void SelectCommand(GameObject commandObject)
  {
    if (commandObject == null)
    {
      Debug.LogError("Failed to select command. commandObject is null!");
      return;
    }

    EventSystem.current.SetSelectedGameObject(commandObject.GetComponentInChildren<Button>().gameObject);
  }
}
