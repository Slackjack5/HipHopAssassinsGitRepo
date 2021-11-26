using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
  [SerializeField] private GameObject menu;

  private readonly List<Consumable> consumables = new List<Consumable>();

  public readonly UnityEvent<Consumable> onPurchase = new UnityEvent<Consumable>();
  public readonly UnityEvent onClose = new UnityEvent();

  private void Start()
  {
    consumables.AddRange(DataManager.AllConsumables);
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
