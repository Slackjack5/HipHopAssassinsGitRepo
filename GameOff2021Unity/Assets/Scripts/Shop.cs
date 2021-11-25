using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shop : Encounter
{
  [SerializeField] private GameObject menu;

  private readonly List<Consumable> consumables = new List<Consumable>();

  public readonly UnityEvent<Consumable> onPurchase = new UnityEvent<Consumable>();
  public readonly UnityEvent onClose = new UnityEvent();

  public void Open()
  {
    menu.SetActive(true);

    var commandLoader = menu.GetComponent<CommandLoader>();
    commandLoader.Load(consumables.ToArray());
    commandLoader.onSubmitCommand.AddListener(command =>
    {
      var consumable = (Consumable) command;
      onPurchase.Invoke(consumable);
    });
  }

  public void Close()
  {
    onClose.Invoke();
  }
}
