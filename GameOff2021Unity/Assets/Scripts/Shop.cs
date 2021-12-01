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
  [SerializeField] private TextMeshProUGUI goldNumber;
  [SerializeField] private GameObject amountOwnedContainer;
  [SerializeField] private TextMeshProUGUI amountOwnedNumber;

  private readonly List<Command> consumables = new List<Command>();

  public readonly UnityEvent<Consumable> onPurchase = new UnityEvent<Consumable>();
  public readonly UnityEvent<int> onClose = new UnityEvent<int>();

  private int currentGold;
  private CommandLoader commandLoader;

  private void Start()
  {
    consumables.AddRange(DataManager.AllConsumables);

    commandLoader = menu.GetComponentInChildren<CommandLoader>();
    commandLoader.onSubmitCommand.AddListener(Purchase);
  }

  private void Update()
  {
    if (!menu.activeSelf) return;

    descriptionPanel.SetActive(true);

    var commandRef = EventSystem.current.currentSelectedGameObject.GetComponent<CommandRef>();
    if (commandRef == null)
    {
      descriptionPanel.SetActive(false);
      amountOwnedContainer.SetActive(false);
    }
    else
    {
      descriptionPanel.GetComponentInChildren<TextMeshProUGUI>().text = commandRef.command.description;

      amountOwnedContainer.SetActive(true);
      amountOwnedNumber.text = $"{((Consumable) commandRef.command).AmountOwned}";
    }

    goldNumber.text = $"${currentGold}";
  }

  public void Open(int gold)
  {
    menu.SetActive(true);
    EventSystem.current.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);
    AudioEvents.SetSwitchShop();
    commandLoader.Load(consumables.ToArray(), true);
    currentGold = gold;
  }

  public void Close()
  {
    menu.SetActive(false);
    onClose.Invoke(currentGold);
    AudioEvents.SetSwitchCombat();
  }

  private void Purchase(Command command)
  {
    var consumable = (Consumable) command;

    if (consumable.cost > currentGold)
    {
      return;
    }

    currentGold -= consumable.cost;

    onPurchase.Invoke(consumable);
  }
}
