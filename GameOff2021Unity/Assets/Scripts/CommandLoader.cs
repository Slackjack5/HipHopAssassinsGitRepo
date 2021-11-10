using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class CommandLoader : MonoBehaviour
{
  [SerializeField] private CombatManager turnManager;
  [SerializeField] private MenuManager menuManager;
  [SerializeField] private GameObject commandPrefab;
  [SerializeField] private GameObject commandPanel;
  [SerializeField] private TextMeshProUGUI pageLabel;
  [SerializeField] private int pageSize = 6;

  private Command[] commands;
  private int currentPage = 1;
  private int totalPages = 0;

  public void LoadCommands(Command[] commands)
  {
    this.commands = commands;

    currentPage = 1;
    totalPages = commands.Length / pageSize;
    if (commands.Length % pageSize > 0)
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

    for (int i = 0; i < pageSize; i++)
    {
      // If currentPage is 0, we want to fetch items 0 to 5.
      // If currentPage is 1, we want to fetch items 6 to 11, and so on.
      int index = i + pageSize * (currentPage - 1);
      if (index < commands.Length)
      {
        GameObject commandObject = Instantiate(commandPrefab, commandPanel.transform);

        if (i == 0)
        {
          SelectCommand(commandObject);
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

        commandObject.GetComponentInChildren<Button>().onClick.AddListener(() => SubmitCommand(commandObject));

        // Index 1 refers to the text itself. Index 0 is the cursor.
        // TODO: Change this to the non-array GetComponent after we use an image for the cursor.
        TextMeshProUGUI textComponent = commandObject.GetComponentsInChildren<TextMeshProUGUI>()[1];
        Command command = commands[i + pageSize * (currentPage - 1)];
        textComponent.text = command.Name;
      }
    }

    SetPageLabel();
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

  private void RegisterPageControl(GameObject commandObject, KeyControl keyControl, UnityAction action)
  {
    GameObject buttonObject = commandObject.GetComponentInChildren<Button>().gameObject;
    CommandPageControl control = buttonObject.AddComponent<CommandPageControl>();
    control.keyControl = keyControl;
    control.activate.AddListener(action);
  }

  private void SelectCommand(GameObject commandObject)
  {
    EventSystem.current.SetSelectedGameObject(commandObject.GetComponentInChildren<Button>().gameObject);
  }

  private void SetPageLabel()
  {
    pageLabel.text = currentPage + " of " + totalPages;
  }

  private void SubmitCommand(GameObject commandObject)
  {
    turnManager.GoNextTurn();
    menuManager.OpenTopMenu();
  }
}
