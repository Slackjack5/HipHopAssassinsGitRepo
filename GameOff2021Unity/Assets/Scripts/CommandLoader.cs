using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandLoader : MonoBehaviour
{
  [SerializeField] private GameObject commandPrefab;
  [SerializeField] private GameObject commandPanel;
  [SerializeField] private TextMeshProUGUI pageLabel;
  [SerializeField] private int pageSize = 6;

  private int currentPage = 1;

  private void Start()
  {
    ClearCommands();

    for (int i = 0; i < pageSize; i++)
    {
      GameObject commandObject = Instantiate(commandPrefab, commandPanel.transform);

      // If currentPage is 0, we want to fetch items 0 to 5.
      // If currentPage is 1, we want to fetch items 6 to 11, and so on.
      Consumable consumable = XmlManager.Consumables[i + pageSize * (currentPage - 1)];

      // Index 1 refers to the text itself. Index 0 is the cursor.
      // TODO: Change this to the non-array GetComponent after we use an image for the cursor.
      TextMeshProUGUI textComponent = commandObject.GetComponentsInChildren<TextMeshProUGUI>()[1];
      textComponent.text = consumable.name;
    }

    int totalPages = XmlManager.Consumables.Length / pageSize;
    if (XmlManager.Consumables.Length % pageSize > 0)
    {
      totalPages++;
    }
    SetPageLabel(totalPages);
  }

  private void ClearCommands()
  {
    foreach (Transform child in commandPanel.transform)
    {
      Destroy(child.gameObject);
    }
  }

  private void SetPageLabel(int totalPages)
  {
    pageLabel.text = currentPage + " of " + totalPages;
  }
}
