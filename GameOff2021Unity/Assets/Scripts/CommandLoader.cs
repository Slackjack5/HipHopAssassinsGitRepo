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

    foreach (Consumable consumable in XmlManager.Consumables)
    {
      GameObject commandObject = Instantiate(commandPrefab, commandPanel.transform);

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
