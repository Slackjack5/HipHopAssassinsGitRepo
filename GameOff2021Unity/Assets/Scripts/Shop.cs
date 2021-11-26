using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
  [SerializeField] private GameObject menu;
  [SerializeField] private GameObject descriptionPanel;

  private readonly List<Consumable> consumables = new List<Consumable>();

  public readonly UnityEvent<Consumable> onPurchase = new UnityEvent<Consumable>();
  public readonly UnityEvent onClose = new UnityEvent();

  private void Start()
  {
    consumables.AddRange(DataManager.AllConsumables);
  }

  private void Update()
  {
    if (menu.activeSelf)
    {
      descriptionPanel.SetActive(true);

      var description = EventSystem.current.currentSelectedGameObject.GetComponent<Description>();
      if (description == null)
      {
        descriptionPanel.SetActive(false);
      }
      else
      {
        descriptionPanel.GetComponentInChildren<TextMeshProUGUI>().text = description.text;
      }
    }
    else
    {
      descriptionPanel.SetActive(false);
    }
  }

  public void Open()
  {
    menu.SetActive(true);
    EventSystem.current.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);

    var commandLoader = menu.GetComponentInChildren<CommandLoader>();
    commandLoader.Load(consumables.ToArray(), true);
    commandLoader.onSubmitCommand.AddListener(command =>
    {
      var consumable = (Consumable) command;
      onPurchase.Invoke(consumable);
    });
  }

  public void Close()
  {
    menu.SetActive(false);
    onClose.Invoke();
  }
}
